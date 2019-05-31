namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Patterns;

    public class EmployeeSickLeaveActor : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly string employeeId;
        private readonly ActorSelection calendarEventsApprovalsChecker;
        private readonly IActorRef sickLeavesRegistry;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EmployeeSickLeaveActor(
            string employeeId,
            IEmployeeSickLeavesRegistryPropsFactory sickLeavesRegistryPropsFactory)
        {
            this.employeeId = employeeId;

            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var sickLeavesRegistryProps = sickLeavesRegistryPropsFactory.CreateProps(employeeId);

            this.sickLeavesRegistry = Context.ActorOf(
                persistenceSupervisorFactory.Get(sickLeavesRegistryProps),
                "sick-leaves-registry");

            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
        }

        public static Props CreateProps(
            string employeeId,
            IEmployeeSickLeavesRegistryPropsFactory sickLeavesRegistryPropsFactory)
        {
            return Props.Create(() => new EmployeeSickLeaveActor(employeeId, sickLeavesRegistryPropsFactory));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventRecoverComplete msg when
                    msg.Event.Type == CalendarEventTypes.Sickleave &&
                    msg.Event.EmployeeId == this.employeeId &&
                    msg.Event.IsPending:

                    this.logger.Debug($"Recover complete for event {msg.Event.EventId}. Event is pending and will be added to pending actions.");

                    this.GetRecoveryCompleteApprover(msg.Event)
                        .PipeTo(
                            this.Self,
                            success: result => new RecoveryCompleteSuccess(msg.Event, result),
                            failure: err => new RecoveryCompleteError(msg.Event, err));

                    break;

                case CalendarEventRecoverComplete msg:
                    if (msg.Event.EmployeeId == this.employeeId)
                    {
                        this.logger.Debug($"Recover complete for event {msg.Event.EventId}. Event is not pending and won't be added to pending actions.");
                    }

                    break;

                case RecoveryCompleteSuccess msg:
                    this.logger.Debug($"Event {msg.Event.EventId}. Next approver is {msg.NextApprover}. Event is pending and will be added to pending actions.");
                    Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                    break;

                case RecoveryCompleteError msg:
                    this.logger.Error(msg.Exception, $"Error occured on event {msg.Event.EventId} recover for employee {this.employeeId}");
                    break;

                case GetCalendarEvents msg:
                    this.sickLeavesRegistry.Forward(msg);
                    break;

                case GetCalendarEvent msg:
                    this.sickLeavesRegistry.Forward(msg);
                    break;

                case GetCalendarEventApprovals msg:
                    this.sickLeavesRegistry.Forward(msg);
                    break;

                case UpsertCalendarEvent msg:
                    this.GetSickLeave(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            sickLeave =>
                            {
                                if (sickLeave == null)
                                {
                                    return new InsertSickLeave(msg.Event, msg.UpdatedBy, msg.Timestamp);
                                }

                                return new UpdateSickLeave(msg.Event, sickLeave, msg.UpdatedBy, msg.Timestamp);
                            },
                            err => new UpsertCalendarEvent.Error(err.ToString()));
                    break;

                case UpsertCalendarEvent.Error msg:
                    this.Sender.Tell(msg);
                    break;

                case InsertSickLeave msg:
                    this.InsertSickLeave(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertSickLeaveSuccess(result),
                            err => new InsertSickLeave.Error(err));
                    break;

                case InsertSickLeaveSuccess msg:
                    this.logger.Debug($"Sick leave {msg.Data.Event.EventId} is created.");

                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Data.Event, msg.Data.CreatedBy, msg.Data.Timestamp));

                    if (msg.Data.NextApprover != null)
                    {
                        this.logger.Debug($"Sick leave {msg.Data.Event.EventId}. Next approver is {msg.Data.NextApprover}. Sick leave will be added to pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Data.Event, msg.Data.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Data.Event, msg.Data.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug($"Sick leave {msg.Data.Event.EventId}. There is no next approver, sick leave won't be added to pending actions.");

                        // If sick leave doesn't require approval and has been approved automatically,
                        // we imply, for simplicity, that it was changed from the same sick leave, but with initial status
                        var oldEvent = new CalendarEvent(
                            msg.Data.Event.EventId,
                            msg.Data.Event.Type,
                            msg.Data.Event.Dates,
                            SickLeaveStatuses.Requested,
                            msg.Data.Event.EmployeeId);
                        Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, msg.Data.Event.EmployeeId, DateTimeOffset.Now, msg.Data.Event));
                    }

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.Data.Event));

                    break;

                case InsertSickLeave.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                case UpdateSickLeave msg:
                    this.UpdateSickLeave(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateSickLeave.Success(result, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateSickLeave.Error(err));
                    break;

                case UpdateSickLeave.Success msg:
                    this.logger.Debug($"Sick leave {msg.NewEvent.EventId} is changed.");

                    Context.System.EventStream.Publish(new CalendarEventChanged(
                        msg.OldEvent,
                        msg.UpdatedBy,
                        msg.Timestamp,
                        msg.NewEvent));

                    if (!msg.NewEvent.IsPending)
                    {
                        this.logger.Debug($"Sick leave {msg.NewEvent.EventId} is not pending and will be removed from current approver pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.NewEvent));
                    }

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.NewEvent));

                    break;

                case UpdateSickLeave.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                case ApproveCalendarEvent msg:
                    this.GetSickLeave(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            sickLeave =>
                            {
                                if (sickLeave != null)
                                {
                                    return new ApproveSickLeave(sickLeave, msg.ApproverId, msg.Timestamp);
                                }

                                return new ApproveCalendarEvent.ErrorResponse($"Sick leave with id {msg.Event.EventId} is not found");
                            },
                            err =>
                            {
                                if (err is ArgumentException)
                                {
                                    return new ApproveCalendarEvent.BadRequestResponse(err.ToString());
                                }

                                return new ApproveCalendarEvent.ErrorResponse(err.ToString());
                            });
                    break;

                case ApproveCalendarEvent.ErrorResponse msg:
                    this.Sender.Tell(msg);
                    break;

                case ApproveCalendarEvent.BadRequestResponse msg:
                    this.Sender.Tell(msg);
                    break;

                case ApproveSickLeave msg:
                    this.GrantSickLeaveApproval(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new ApproveSickLeaveSuccess(result),
                            err => new ApproveSickLeave.Error(err));
                    break;

                case ApproveSickLeaveSuccess msg:
                    if (msg.Data.NewEvent != null)
                    {
                        this.logger.Debug($"Approval is granted for sick leave {msg.Data.NewEvent.EventId}.");

                        if (msg.Data.NextApprover != null)
                        {
                            this.logger.Debug($"Sick leave {msg.Data.NewEvent.EventId}. Next approver is {msg.Data.NextApprover}. Sick leave will be added to pending actions.");

                            Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Data.NewEvent, msg.Data.Approvals.ToList()));

                            Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Data.NewEvent, msg.Data.NextApprover));
                            Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Data.NewEvent, msg.Data.NextApprover));
                        }
                        else
                        {
                            this.logger.Debug($"Sick leave {msg.Data.NewEvent.EventId}. There is no next approver, sick leave will be removed from current approver pending actions.");

                            Context.System.EventStream.Publish(
                                new CalendarEventChanged(
                                    msg.Data.OldEvent,
                                    msg.Data.ApprovedBy,
                                    msg.Data.Timestamp,
                                    msg.Data.NewEvent));

                            Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.Data.NewEvent));
                        }
                    }

                    this.Sender.Tell(ApproveCalendarEvent.SuccessResponse.Instance);

                    break;

                case ApproveSickLeave.Error msg:
                    this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(msg.Exception.ToString()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<string> GetRecoveryCompleteApprover(CalendarEvent @event)
        {
            var approvals = await this.GetApprovals(@event);
            var nextApprover = await this.GetNextApproverId(@event, approvals);
            return nextApprover;
        }

        private async Task<CalendarEvent> GetSickLeave(string eventId)
        {
            var message = new GetCalendarEvent(eventId);
            var response = await this.sickLeavesRegistry.Ask<GetCalendarEvent.Response>(message);

            if (response is GetCalendarEvent.Response.Found success)
            {
                return success.Event;
            }

            return null;
        }

        private async Task<IEnumerable<Approval>> GetApprovals(CalendarEvent @event)
        {
            var message = new GetCalendarEventApprovals(@event);
            var response = await this.sickLeavesRegistry.Ask<GetCalendarEventApprovals.Response>(message);

            switch (response)
            {
                case GetCalendarEventApprovals.SuccessResponse success:
                    return success.Approvals;

                case GetCalendarEventApprovals.ErrorResponse error:
                    throw new Exception(error.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<InsertSickLeaveSuccessData> InsertSickLeave(InsertSickLeave message)
        {
            await this.EnsureInsertAvailable(message.Event);

            var response = await this.sickLeavesRegistry.Ask<InsertSickLeave.Response>(message);

            switch (response)
            {
                case InsertSickLeave.Success success:
                    var @event = success.Event;

                    var nextApprover = await this.GetNextApproverId(success.Event, Enumerable.Empty<Approval>());

                    if (nextApprover == null)
                    {
                        @event = await this.CompleteSickLeave(@event, message.CreatedBy, message.Timestamp);
                    }

                    return new InsertSickLeaveSuccessData(@event, message.CreatedBy, message.Timestamp, nextApprover);

                case InsertSickLeave.Error error:
                    throw new Exception("Error occured on sick leave insert", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> UpdateSickLeave(UpdateSickLeave message, bool needCheckUpdateAvailable = true)
        {
            // When sick leave is changed from internal code (change to Approved when all approvals granted), availability should not be checked
            if (needCheckUpdateAvailable)
            {
                await this.EnsureUpdateAvailable(message.NewEvent, message.OldEvent);
            }

            var response = await this.sickLeavesRegistry.Ask<UpdateSickLeave.Response>(message);

            switch (response)
            {
                case UpdateSickLeave.Success success:
                    return success.NewEvent;

                case UpdateSickLeave.Error error:
                    throw new Exception("Error occured on sick leave update", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<ApproveSickLeaveSuccessData> GrantSickLeaveApproval(ApproveSickLeave message)
        {
            this.EnsureApprovalAvailable(message.Event);

            var response = await this.sickLeavesRegistry.Ask<ApproveSickLeave.Response>(message);

            switch (response)
            {
                case ApproveSickLeave.Success success:
                    string nextApprover = null;
                    var newEvent = success.Event;

                    if (success.Event != null)
                    {
                        nextApprover = await this.GetNextApproverId(success.Event, success.Approvals);

                        if (nextApprover == null)
                        {
                            newEvent = await this.CompleteSickLeave(newEvent, message.ApprovedBy, message.Timestamp);
                        }
                    }

                    return new ApproveSickLeaveSuccessData(
                        newEvent,
                        message.Event,
                        success.Approvals,
                        message.ApprovedBy,
                        message.Timestamp,
                        nextApprover);

                case ApproveSickLeave.Error error:
                    throw new Exception("Error occured on sick leave approval granted", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> CompleteSickLeave(CalendarEvent @event, string completeBy, DateTimeOffset timestamp)
        {
            var approvedStatus = new CalendarEventStatuses().ApprovedForType(@event.Type);

            var newEvent = new CalendarEvent(
                @event.EventId,
                @event.Type,
                @event.Dates,
                approvedStatus,
                @event.EmployeeId
            );

            var updateMessage = new UpdateSickLeave(newEvent, @event, completeBy, timestamp);
            var updatedSickLeave = await this.UpdateSickLeave(updateMessage, false);

            return updatedSickLeave;
        }

        private async Task<string> GetNextApproverId(CalendarEvent @event, IEnumerable<Approval> approvals)
        {
            var approvedBy = approvals.Select(a => a.ApprovedBy);

            var response = await this.calendarEventsApprovalsChecker
                .Ask<GetNextCalendarEventApprover.Response>(
                    new GetNextCalendarEventApprover(@event.EmployeeId, approvedBy, @event.Type));

            switch (response)
            {
                case GetNextCalendarEventApprover.SuccessResponse msg:
                    return msg.NextApproverEmployeeId;

                case GetNextCalendarEventApprover.ErrorResponse msg:
                    throw new Exception(msg.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task EnsureInsertAvailable(CalendarEvent @event)
        {
            if (@event.Status != this.GetInitialStatus())
            {
                throw new Exception($"Event {@event.EventId}. Initial status must be {this.GetInitialStatus()}");
            }

            await this.EnsureDatesAreNotIntersected(@event);
        }

        private async Task EnsureUpdateAvailable(CalendarEvent newEvent, CalendarEvent oldEvent)
        {
            if (oldEvent.Status != newEvent.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, newEvent.Status))
            {
                throw new Exception(
                    $"Event {newEvent.EventId}. Status transition {oldEvent.Status} -> {newEvent.Status} " +
                    "is not allowed for sick leave");
            }

            if (oldEvent.Dates.StartDate != newEvent.Dates.StartDate)
            {
                throw new Exception("Start date cannot be changed");
            }

            await this.EnsureDatesAreNotIntersected(newEvent);
        }

        private void EnsureApprovalAvailable(CalendarEvent @event)
        {
            if (!@event.IsPending)
            {
                throw new ArgumentException($"Approval of non-pending event with id {@event.EventId} is not allowed");
            }
        }

        private async Task EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var message = new CheckDatesAvailability(@event);
            var response = await this.sickLeavesRegistry.Ask<CheckDatesAvailability.Response>(message);

            switch (response)
            {
                case CheckDatesAvailability.Success success:

                    if (!success.Result)
                    {
                        throw new Exception($"Event {@event.EventId}. Dates intersect with another actual sick leave");
                    }

                    return;

                case CheckDatesAvailability.Error error:
                    throw new Exception("Error occured on dates availability check", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private string GetInitialStatus()
        {
            return SickLeaveStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return SickLeaveStatuses.All.Contains(newCalendarEventStatus)
                && oldCalendarEventStatus != SickLeaveStatuses.Cancelled
                && oldCalendarEventStatus != SickLeaveStatuses.Completed
                && newCalendarEventStatus != SickLeaveStatuses.Requested
                && newCalendarEventStatus != SickLeaveStatuses.Approved;
        }

        private class RecoveryCompleteSuccess
        {
            public RecoveryCompleteSuccess(CalendarEvent @event, string nextApprover)
            {
                this.Event = @event;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public string NextApprover { get; }
        }

        private class RecoveryCompleteError
        {
            public RecoveryCompleteError(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }

        private class InsertSickLeaveSuccessData
        {
            public InsertSickLeaveSuccessData(CalendarEvent @event, string createdBy, DateTimeOffset timestamp, string nextApprover)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public string NextApprover { get; }
        }

        private class InsertSickLeaveSuccess
        {
            public InsertSickLeaveSuccess(InsertSickLeaveSuccessData data)
            {
                this.Data = data;
            }

            public InsertSickLeaveSuccessData Data { get; }
        }

        private class ApproveSickLeaveSuccessData
        {
            public ApproveSickLeaveSuccessData(
                CalendarEvent newEvent,
                CalendarEvent oldEvent,
                IEnumerable<Approval> approvals,
                string approvedBy,
                DateTimeOffset timestamp,
                string nextApprover)
            {
                this.NewEvent = newEvent;
                this.OldEvent = oldEvent;
                this.Approvals = approvals;
                this.ApprovedBy = approvedBy;
                this.Timestamp = timestamp;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent NewEvent { get; }

            public CalendarEvent OldEvent { get; }

            public IEnumerable<Approval> Approvals { get; }

            public string ApprovedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public string NextApprover { get; }
        }

        private class ApproveSickLeaveSuccess
        {
            public ApproveSickLeaveSuccess(ApproveSickLeaveSuccessData data)
            {
                this.Data = data;
            }

            public ApproveSickLeaveSuccessData Data { get; }
        }
    }
}
namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Patterns;

    public class EmployeeVacationsActor : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly string employeeId;
        private readonly IActorRef employeeFeed;
        private readonly IActorRef vacationsCreditRegistry;
        private readonly ActorSelection calendarEventsApprovalsChecker;
        private readonly IActorRef vacationsRegistry;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EmployeeVacationsActor(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory
        )
        {
            this.employeeId = employeeId;
            this.employeeFeed = employeeFeed;
            this.vacationsCreditRegistry = vacationsCreditRegistry;

            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var vacationsRegistryProps = vacationsRegistryPropsFactory.CreateProps(employeeId);

            this.vacationsRegistry = Context.ActorOf(
                persistenceSupervisorFactory.Get(vacationsRegistryProps),
                "vacations-registry");

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
        }

        public static Props CreateProps(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory)
        {
            return Props.Create(() => new EmployeeVacationsActor(
                employeeId,
                employeeFeed,
                vacationsCreditRegistry,
                vacationsRegistryPropsFactory)
            );
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when
                    msg.NewEvent.Type == CalendarEventTypes.Vacation &&
                    msg.NewEvent.Status == VacationStatuses.Processed &&
                    msg.NewEvent.EmployeeId == this.employeeId:

                    var text = $"Vacation approved from {msg.NewEvent.Dates.StartDate.ToLongDateString()} to {msg.NewEvent.Dates.EndDate.ToLongDateString()}";
                    var feedMessage = new Message(Guid.NewGuid().ToString(), this.employeeId, "Vacation", text, msg.Timestamp.Date);
                    this.employeeFeed.Tell(new PostMessage(feedMessage));

                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventRecoverComplete msg when
                    msg.Event.Type == CalendarEventTypes.Vacation &&
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

                case GetVacationsCredit _:
                    this.vacationsCreditRegistry
                        .Ask<VacationsCreditRegistry.GetVacationInfo.Response>(new VacationsCreditRegistry.GetVacationInfo(this.employeeId))
                        .ContinueWith(x => new GetVacationsCredit.Response(x.Result.VacationsCredit))
                        .PipeTo(this.Sender);
                    break;

                case GetCalendarEvents msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEvent msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEventApprovals msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case UpsertCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            vacation =>
                            {
                                if (vacation == null)
                                {
                                    return new InsertVacation(msg.Event, msg.UpdatedBy, msg.Timestamp);
                                }

                                return new UpdateVacation(msg.Event, vacation, msg.UpdatedBy, msg.Timestamp);
                            },
                            err => new UpsertCalendarEvent.Error(err.ToString()));
                    break;

                case UpsertCalendarEvent.Error msg:
                    this.Sender.Tell(msg);
                    break;

                case InsertVacation msg:
                    this.InsertVacation(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertVacationSuccess(result),
                            err => new InsertVacation.Error(err));
                    break;

                case InsertVacationSuccess msg:
                    this.logger.Debug($"Vacation {msg.Data.Event.EventId} is created.");

                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Data.Event, msg.Data.CreatedBy, msg.Data.Timestamp));

                    if (msg.Data.NextApprover != null)
                    {
                        this.logger.Debug($"Vacation {msg.Data.Event.EventId}. Next approver is {msg.Data.NextApprover}. Vacation will be added to pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Data.Event, msg.Data.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Data.Event, msg.Data.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug($"Vacation {msg.Data.Event.EventId}. There is no next approver, vacation won't be added to pending actions.");

                        // If vacation doesn't require approval and has been approved automatically,
                        // we imply, for simplicity, that it was changed from the same vacation, but with initial status
                        var oldEvent = new CalendarEvent(
                            msg.Data.Event.EventId,
                            msg.Data.Event.Type,
                            msg.Data.Event.Dates,
                            VacationStatuses.Requested,
                            msg.Data.Event.EmployeeId);
                        Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, msg.Data.Event.EmployeeId, DateTimeOffset.Now, msg.Data.Event));
                    }

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.Data.Event));

                    break;

                case InsertVacation.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                case UpdateVacation msg:
                    this.UpdateVacation(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateVacation.Success(result, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateVacation.Error(err));
                    break;

                case UpdateVacation.Success msg:
                    this.logger.Debug($"Vacation {msg.NewEvent.EventId} is changed.");

                    Context.System.EventStream.Publish(new CalendarEventChanged(
                        msg.OldEvent,
                        msg.UpdatedBy,
                        msg.Timestamp,
                        msg.NewEvent));

                    if (!msg.NewEvent.IsPending)
                    {
                        this.logger.Debug($"Vacation {msg.NewEvent.EventId} is not pending and will be removed from current approver pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.NewEvent));
                    }

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.NewEvent));

                    break;

                case UpdateVacation.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                case ApproveCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            vacation =>
                            {
                                if (vacation != null)
                                {
                                    return new ApproveVacation(vacation, msg.ApproverId, msg.Timestamp);
                                }

                                return new ApproveCalendarEvent.ErrorResponse($"Vacation with id {msg.Event.EventId} is not found");
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

                case ApproveVacation msg:
                    this.GrantVacationApproval(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new ApproveVacationSuccess(result),
                            err => new ApproveVacation.Error(err));
                    break;

                case ApproveVacationSuccess msg:
                    if (msg.Data.NewEvent != null)
                    {
                        this.logger.Debug($"Approval is granted for vacation {msg.Data.NewEvent.EventId}.");

                        if (msg.Data.NextApprover != null)
                        {
                            this.logger.Debug($"Vacation {msg.Data.NewEvent.EventId}. Next approver is {msg.Data.NextApprover}. Vacation will be added to pending actions.");

                            Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Data.NewEvent, msg.Data.Approvals.ToList()));

                            Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Data.NewEvent, msg.Data.NextApprover));
                            Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Data.NewEvent, msg.Data.NextApprover));
                        }
                        else
                        {
                            this.logger.Debug($"Vacation {msg.Data.NewEvent.EventId}. There is no next approver, vacation will be removed from current approver pending actions.");

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

                case ApproveVacation.Error msg:
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

        private async Task<CalendarEvent> GetVacation(string eventId)
        {
            var message = new GetCalendarEvent(eventId);
            var response = await this.vacationsRegistry.Ask<GetCalendarEvent.Response>(message);

            if (response is GetCalendarEvent.Response.Found success)
            {
                return success.Event;
            }

            return null;
        }

        private async Task<IEnumerable<Approval>> GetApprovals(CalendarEvent @event)
        {
            var message = new GetCalendarEventApprovals(@event);
            var response = await this.vacationsRegistry.Ask<GetCalendarEventApprovals.Response>(message);

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

        private async Task<InsertVacationSuccessData> InsertVacation(InsertVacation message)
        {
            await this.EnsureInsertAvailable(message.Event);

            var response = await this.vacationsRegistry.Ask<InsertVacation.Response>(message);

            switch (response)
            {
                case InsertVacation.Success success:
                    var @event = success.Event;

                    var nextApprover = await this.GetNextApproverId(success.Event, Enumerable.Empty<Approval>());
                    if (nextApprover == null)
                    {
                        @event = await this.CompleteVacation(@event, message.CreatedBy, message.Timestamp);
                    }

                    return new InsertVacationSuccessData(@event, message.CreatedBy, message.Timestamp, nextApprover);

                case InsertVacation.Error error:
                    throw new Exception("Error occured on vacation insert", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> UpdateVacation(UpdateVacation message, bool needCheckUpdateAvailable = true)
        {
            // When vacation is changed from internal code (change to Approved when all approvals granted), availability should not be checked
            if (needCheckUpdateAvailable)
            {
                await this.EnsureUpdateAvailable(message.NewEvent, message.OldEvent);
            }

            var response = await this.vacationsRegistry.Ask<UpdateVacation.Response>(message);

            switch (response)
            {
                case UpdateVacation.Success success:
                    return success.NewEvent;

                case UpdateVacation.Error error:
                    throw new Exception("Error occured on vacation update", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<ApproveVacationSuccessData> GrantVacationApproval(ApproveVacation message)
        {
            this.EnsureApprovalAvailable(message.Event);

            var response = await this.vacationsRegistry.Ask<ApproveVacation.Response>(message);

            switch (response)
            {
                case ApproveVacation.Success success:
                    string nextApprover = null;
                    CalendarEvent newEvent = success.Event;

                    if (success.Event != null)
                    {
                        nextApprover = await this.GetNextApproverId(success.Event, success.Approvals);
                        if (nextApprover == null)
                        {
                            newEvent = await this.CompleteVacation(newEvent, message.ApprovedBy, message.Timestamp);
                        }
                    }

                    return new ApproveVacationSuccessData(
                        newEvent,
                        message.Event,
                        success.Approvals,
                        message.ApprovedBy,
                        message.Timestamp,
                        nextApprover);

                case ApproveVacation.Error error:
                    throw new Exception("Error occured on vacation approval granted", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> CompleteVacation(CalendarEvent @event, string completeBy, DateTimeOffset timestamp)
        {
            var approvedStatus = new CalendarEventStatuses().ApprovedForType(@event.Type);
            var newEvent = new CalendarEvent(
                @event.EventId,
                @event.Type,
                @event.Dates,
                approvedStatus,
                @event.EmployeeId
            );

            var updateMessage = new UpdateVacation(newEvent, @event, completeBy, timestamp);
            var updatedVacation = await this.UpdateVacation(updateMessage, false);

            return updatedVacation;
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
                    "is not allowed for vacation");
            }

            await this.EnsureDatesAreNotIntersected(newEvent);

            if (oldEvent.Dates != newEvent.Dates)
            {
                if (!oldEvent.IsPending)
                {
                    throw new Exception($"Date change is not allowed in status {oldEvent.Status} for {oldEvent.Type}");
                }

                var approvals = await this.GetApprovals(oldEvent);
                if (approvals.Count() != 0)
                {
                    throw new Exception($"Date change is not allowed when there is at least one user approval for {oldEvent.Type}");
                }
            }
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
            var response = await this.vacationsRegistry.Ask<CheckDatesAvailability.Response>(message);

            switch (response)
            {
                case CheckDatesAvailability.Success success:
                    if (!success.Result)
                    {
                        throw new Exception($"Event {@event.EventId}. Dates intersect with another actual vacation");
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
            return VacationStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return VacationStatuses.All.Contains(newCalendarEventStatus)
                && oldCalendarEventStatus != VacationStatuses.Cancelled
                && oldCalendarEventStatus != VacationStatuses.Rejected
                && oldCalendarEventStatus != VacationStatuses.AccountingReady
                && oldCalendarEventStatus != VacationStatuses.Processed
                && newCalendarEventStatus != VacationStatuses.Requested
                && newCalendarEventStatus != VacationStatuses.Approved
                && newCalendarEventStatus != VacationStatuses.AccountingReady
                && newCalendarEventStatus != VacationStatuses.Processed;
        }

        private class InsertVacationSuccessData
        {
            public InsertVacationSuccessData(CalendarEvent @event, string createdBy, DateTimeOffset timestamp, string nextApprover)
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

        private class InsertVacationSuccess
        {
            public InsertVacationSuccess(InsertVacationSuccessData data)
            {
                this.Data = data;
            }

            public InsertVacationSuccessData Data { get; }
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

        private class ApproveVacationSuccessData
        {
            public ApproveVacationSuccessData(
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

        private class ApproveVacationSuccess
        {
            public ApproveVacationSuccess(ApproveVacationSuccessData data)
            {
                this.Data = data;
            }

            public ApproveVacationSuccessData Data { get; }
        }
    }
}
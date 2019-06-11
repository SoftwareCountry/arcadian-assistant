namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public abstract class CalendarEventsStorageBase : UntypedPersistentActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        protected delegate void OnSuccessfulUpsertCallback(CalendarEvent changedEvent);

        protected string EmployeeId { get; }

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly ActorSelection calendarEventsApprovalsChecker;

        protected readonly Dictionary<string, CalendarEvent> EventsById = new Dictionary<string, CalendarEvent>();
        protected readonly Dictionary<string, List<Approval>> ApprovalsByEvent = new Dictionary<string, List<Approval>>();

        protected CalendarEventsStorageBase(string employeeId)
        {
            this.EmployeeId = employeeId;

            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case RecoveryCompleteSuccess msg:
                    this.logger.Debug($"Event {msg.Event.EventId}. Next approver is {msg.NextApprover}. Event is pending and will be added to pending actions.");
                    Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                    break;

                case RecoveryCompleteError msg:
                    this.logger.Error(msg.Exception, $"Error occured on event {msg.Event.EventId} recover for employee {this.EmployeeId}");
                    break;

                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.EmployeeId, this.EventsById.Values.ToList()));
                    break;

                case GetCalendarEvent request when !this.EventsById.ContainsKey(request.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEvent request:
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.EventsById[request.EventId]));
                    break;

                case UpsertCalendarEvent cmd when cmd.Event.EventId == null:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(new ArgumentNullException(nameof(cmd.Event.EventId)).ToString()));
                    break;

                //insert
                case UpsertCalendarEvent cmd when !this.EventsById.ContainsKey(cmd.Event.EventId):
                    try
                    {
                        if (cmd.Event.Status != this.GetInitialStatus())
                        {
                            throw new Exception($"Event {cmd.Event.EventId}. Initial status must be {this.GetInitialStatus()}");
                        }

                        this.EnsureDatesAreNotIntersected(cmd.Event);

                        this.InsertCalendarEvent(cmd.Event, cmd.UpdatedBy, cmd.Timestamp, ev =>
                        {
                            this.GetNextApproverId(ev, Enumerable.Empty<Approval>())
                                .PipeTo(
                                    this.Self,
                                    this.Sender,
                                    result => new InsertCalendarEventSuccess(ev, cmd.UpdatedBy, cmd.Timestamp, result),
                                    err => new InsertCalendarEventError(err));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.ToString()));
                    }

                    break;

                case InsertCalendarEventSuccess msg:
                    this.logger.Debug($"Event {msg.Event.EventId} is created.");

                    var insertResult = msg.Event;

                    if (msg.NextApprover != null)
                    {
                        this.logger.Debug($"Event {msg.Event.EventId}. Next approver is {msg.NextApprover}. Event will be added to pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug($"Event {msg.Event.EventId}. There is no next approver, event won't be added to pending actions.");

                        insertResult = this.CompleteCalendarEvent(msg.Event, msg.CreatedBy, msg.Timestamp);
                    }

                    Context.System.EventStream.Publish(new CalendarEventCreated(insertResult, msg.CreatedBy, msg.Timestamp));

                    if (!insertResult.IsPending)
                    {
                        // If event doesn't require approval and has been approved automatically,
                        // we imply, for simplicity, that it was changed from the same event, but with initial status
                        var oldEvent = new CalendarEvent(
                            insertResult.EventId,
                            insertResult.Type,
                            insertResult.Dates,
                            WorkHoursChangeStatuses.Requested,
                            insertResult.EmployeeId);
                        Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, insertResult.EmployeeId, DateTimeOffset.Now, insertResult));
                    }

                    this.Sender.Tell(new UpsertCalendarEvent.Success(insertResult));

                    break;

                case InsertCalendarEventError msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                //update
                case UpsertCalendarEvent cmd:
                    this.logger.Debug($"Event {cmd.Event.EventId} is changed.");

                    try
                    {
                        var oldEvent = this.EventsById[cmd.Event.EventId];

                        if (oldEvent.Status != cmd.Event.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, cmd.Event.Status))
                        {
                            throw new Exception($"Event {cmd.Event.EventId}. Status transition {oldEvent.Status} -> {cmd.Event.Status} " +
                                $"is not allowed for {oldEvent.Type}");
                        }

                        this.EnsureDatesAreNotIntersected(cmd.Event);

                        this.UpdateCalendarEvent(oldEvent, cmd.UpdatedBy, cmd.Timestamp, cmd.Event, ev =>
                        {
                            Context.System.EventStream.Publish(new CalendarEventChanged(
                                oldEvent,
                                cmd.UpdatedBy,
                                cmd.Timestamp,
                                ev));

                            if (!ev.IsPending)
                            {
                                this.logger.Debug($"Event {cmd.Event.EventId} is not pending and will be removed from current approver pending actions.");
                                Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(ev));
                            }

                            this.Sender.Tell(new UpsertCalendarEvent.Success(ev));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.ToString()));
                    }

                    break;

                case GetCalendarEventApprovals msg:
                    if (!this.ApprovalsByEvent.TryGetValue(msg.Event.EventId, out var approvals))
                    {
                        this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Event with event id {msg.Event.EventId} is not found"));
                    }

                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(approvals));
                    break;

                case ApproveCalendarEvent msg:
                    this.logger.Debug($"Approval is granted for event {msg.Event.EventId}.");

                    try
                    {
                        this.ApproveCalendarEvent(msg);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(ex.ToString()));
                    }

                    break;

                case ApproveCalendarEventSuccess msg:
                    if (msg.NextApprover != null)
                    {
                        this.logger.Debug($"Event {msg.Event.EventId}. Next approver is {msg.NextApprover}. Event will be added to pending actions.");

                        Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Event, msg.Approvals.ToList()));

                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug($"Event {msg.Event.EventId}. There is no next approver, event will be removed from current approver pending actions.");

                        var approveResult = this.CompleteCalendarEvent(msg.Event, msg.UpdatedBy, msg.Timestamp);

                        Context.System.EventStream.Publish(
                            new CalendarEventChanged(
                                msg.Event,
                                msg.UpdatedBy,
                                msg.Timestamp,
                                approveResult));

                        Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.Event));
                    }

                    this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);

                    break;

                case ApproveCalendarEventError msg:
                    this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(msg.Exception.ToString()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected void OnRecoveryComplete(IEnumerable<CalendarEvent> pendingEvents)
        {
            foreach (var @event in pendingEvents)
            {
                this.logger.Debug($"Recover complete for event {@event.EventId}. Event is pending and will be added to pending actions.");

                var approvals = this.ApprovalsByEvent[@event.EventId];

                this.GetNextApproverId(@event, approvals)
                    .PipeTo(
                        this.Self,
                        success: result => new RecoveryCompleteSuccess(@event, result),
                        failure: err => new RecoveryCompleteError(@event, err));
            }
        }

        protected abstract void InsertCalendarEvent(CalendarEvent calendarEvent, string updatedBy, DateTimeOffset timestamp, OnSuccessfulUpsertCallback onUpsert);

        protected abstract void UpdateCalendarEvent(CalendarEvent oldEvent, string updatedBy, DateTimeOffset timestamp, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert);

        protected abstract string GetInitialStatus();

        protected abstract bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus);

        protected virtual void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var approvals = this.ApprovalsByEvent[message.EventId];
            approvals.Add(new Approval(message.TimeStamp, message.UserId));
        }

        private void ApproveCalendarEvent(ApproveCalendarEvent message)
        {
            var calendarEvent = this.EventsById[message.Event.EventId];
            var approvals = this.ApprovalsByEvent[message.Event.EventId];

            if (!calendarEvent.IsPending)
            {
                var errorMessage = $"Approval of non-pending event {message.Event} is not allowed";
                this.Sender.Tell(new ApproveCalendarEvent.BadRequestResponse(errorMessage));
                return;
            }

            if (approvals.Any(a => a.ApprovedBy == message.ApproverId))
            {
                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                return;
            }

            var @event = new UserGrantedCalendarEventApproval
            {
                EventId = message.Event.EventId,
                TimeStamp = message.Timestamp,
                UserId = message.ApproverId
            };

            this.OnSuccessfulApprove(@event);

            var newApprovals = this.ApprovalsByEvent[message.Event.EventId];

            this.Persist(@event, ev =>
            {
                this.GetNextApproverId(message.Event, approvals)
                    .PipeTo(
                        this.Self,
                        this.Sender,
                        result => new ApproveCalendarEventSuccess(
                            message.Event,
                            newApprovals.ToList(),
                            message.ApproverId,
                            message.Timestamp,
                            result),
                        err => new ApproveCalendarEventError(err));
            });
        }

        private CalendarEvent CompleteCalendarEvent(CalendarEvent @event, string completeBy, DateTimeOffset timestamp)
        {
            var approvedStatus = new CalendarEventStatuses().ApprovedForType(@event.Type);
            var newEvent = new CalendarEvent(
                @event.EventId,
                @event.Type,
                @event.Dates,
                approvedStatus,
                @event.EmployeeId
            );

            this.UpdateCalendarEvent(@event, completeBy, timestamp, newEvent, ev => { });
            return newEvent;
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

        private void EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var intersectedEvent = this.EventsById.Values
                .Where(ev => ev.EventId != @event.EventId && ev.Type == @event.Type)
                .FirstOrDefault(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            if (intersectedEvent != null)
            {
                throw new Exception($"Event {@event.EventId}. Dates intersect with another {@event.Type} with id {intersectedEvent.EventId}");
            }
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

        private class InsertCalendarEventSuccess
        {
            public InsertCalendarEventSuccess(
                CalendarEvent @event,
                string createdBy,
                DateTimeOffset timestamp,
                string nextApprover)
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

        private class InsertCalendarEventError
        {
            public InsertCalendarEventError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }

        private class ApproveCalendarEventSuccess
        {
            public ApproveCalendarEventSuccess(
                CalendarEvent @event,
                IEnumerable<Approval> approvals,
                string updatedBy,
                DateTimeOffset timestamp,
                string nextApprover)
            {
                this.Event = @event;
                this.Approvals = approvals;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public IEnumerable<Approval> Approvals { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public string NextApprover { get; }
        }

        private class ApproveCalendarEventError
        {
            public ApproveCalendarEventError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
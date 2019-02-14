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
    using Arcadia.Assistant.Calendar.Events;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public abstract class CalendarEventsStorageBase : UntypedPersistentActor, ILogReceive
    {
        protected delegate void OnSuccessfulUpsertCallback(CalendarEvent changedEvent);

        protected string EmployeeId { get; }

        protected readonly Dictionary<string, CalendarEvent> EventsById = new Dictionary<string, CalendarEvent>();
        protected readonly Dictionary<string, List<Approval>> ApprovalsByEvent = new Dictionary<string, List<Approval>>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef calendarEventsApprovalsChecker;

        protected CalendarEventsStorageBase(string employeeId, IActorRef calendarEventsApprovalsChecker)
        {
            this.EmployeeId = employeeId;
            this.calendarEventsApprovalsChecker = calendarEventsApprovalsChecker;
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
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
                    this.Sender.Tell(new UpsertCalendarEvent.Error(new ArgumentNullException(nameof(cmd.Event.EventId)).Message));
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
                            this.OnSuccessfulUpsert(ev);
                            Context.System.EventStream.Publish(new CalendarEventCreated(ev, cmd.UpdatedBy, cmd.Timestamp));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                    }
                    break;

                //update
                case UpsertCalendarEvent cmd:
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
                            this.OnSuccessfulUpsert(ev);
                            Context.System.EventStream.Publish(new CalendarEventChanged(
                                oldEvent,
                                cmd.UpdatedBy,
                                cmd.Timestamp,
                                ev));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
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
                    try
                    {
                        this.ApproveCalendarEvent(msg);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(ex.Message));
                    }

                    break;

                case AssignCalendarEventNextApprover msg:
                    this.GetNextApproverId(msg.EventId)
                        .PipeTo(
                            this.Self,
                            success: x => new AssignCalendarEventNextApprover.Success(msg.EventId, x),
                            failure: err => new AssignCalendarEventNextApprover.Error(msg.EventId, err.Message));
                    break;

                case AssignCalendarEventNextApprover.Success msg:
                    this.OnCalendarEventNextApproverReceived(msg.EventId, msg.NextApproverId);
                    break;

                case AssignCalendarEventNextApprover.Error msg:
                    this.logger.Warning($"Failed to get next approver for the event {msg.EventId}");
                    break;

                case AddCalendarEventToPendingActions msg:
                    this.GetNextApproverId(msg.EventId)
                        .PipeTo(
                            this.Self,
                            success: x => new AddCalendarEventToPendingActions.Success(msg.EventId, x),
                            failure: err => new AddCalendarEventToPendingActions.Error(msg.EventId, err.Message));
                    break;

                case AddCalendarEventToPendingActions.Success msg:
                    this.OnCalendarEventRecoveryComplete(msg.EventId, msg.NextApproverId);
                    break;

                case AddCalendarEventToPendingActions.Error msg:
                    this.logger.Warning($"Failed to get next approver for the event {msg.EventId}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
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

            this.Persist(@event, ev =>
            {
                this.OnSuccessfulApprove(ev);

                this.Self.Tell(new AssignCalendarEventNextApprover(message.Event.EventId));
                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(calendarEvent, approvals.ToList()));
            });
        }

        private void OnCalendarEventNextApproverReceived(string eventId, string nextApproverId)
        {
            var oldEvent = this.EventsById[eventId];
            if (!oldEvent.IsPending)
            {
                return;
            }

            if (nextApproverId == null)
            {
                var approvedStatus = new CalendarEventStatuses().ApprovedForType(oldEvent.Type);
                var newEvent = new CalendarEvent(
                    oldEvent.EventId,
                    oldEvent.Type,
                    oldEvent.Dates,
                    approvedStatus,
                    oldEvent.EmployeeId
                );

                var approvals = this.ApprovalsByEvent[eventId];

                var lastApproval = approvals
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefault();

                // If there is no approvals, then employee is Director General and event is updated by himself
                var updatedBy = lastApproval?.ApprovedBy ?? newEvent.EmployeeId;
                var timestamp = lastApproval?.Timestamp ?? DateTimeOffset.Now;

                this.UpdateCalendarEvent(oldEvent, updatedBy, timestamp, newEvent, ev =>
                {
                    Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, updatedBy, timestamp, ev));
                });
            }

            Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(oldEvent, nextApproverId));
            Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(oldEvent, nextApproverId));
        }

        private void OnCalendarEventRecoveryComplete(string eventId, string nextApproverId)
        {
            var oldEvent = this.EventsById[eventId];
            if (!oldEvent.IsPending || nextApproverId == null)
            {
                return;
            }

            Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(oldEvent, nextApproverId));
        }

        private void OnSuccessfulUpsert(CalendarEvent calendarEvent)
        {
            this.Sender.Tell(new UpsertCalendarEvent.Success(calendarEvent));

            if (calendarEvent.IsPending)
            {
                this.Self.Tell(new AssignCalendarEventNextApprover(calendarEvent.EventId));
            }
            else
            {
                Context.System.EventStream.Publish(new CalendarEventRemovedFromApprovers(calendarEvent));
                Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(calendarEvent));
            }
        }

        private async Task<string> GetNextApproverId(string eventId)
        {
            var calendarEvent = this.EventsById[eventId];
            var existingApprovals = this.ApprovalsByEvent[eventId].Select(a => a.ApprovedBy);

            var response = await this.calendarEventsApprovalsChecker
                .Ask<GetNextCalendarEventApprover.Response>(
                    new GetNextCalendarEventApprover(this.EmployeeId, existingApprovals, calendarEvent.Type));

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

        protected class AssignCalendarEventNextApprover
        {
            public AssignCalendarEventNextApprover(string eventId)
            {
                this.EventId = eventId;
            }

            public string EventId { get; }

            public abstract class Response
            {
            }

            public class Success : Response
            {
                public Success(string eventId, string nextApproverId)
                {
                    this.EventId = eventId;
                    this.NextApproverId = nextApproverId;
                }

                public string EventId { get; }

                public string NextApproverId { get; }
            }

            public class Error : Response
            {
                public Error(string eventId, string message)
                {
                    this.EventId = eventId;
                    this.Message = message;
                }

                public string EventId { get; }

                public string Message { get; }
            }
        }

        protected class AddCalendarEventToPendingActions
        {
            public AddCalendarEventToPendingActions(string eventId)
            {
                this.EventId = eventId;
            }

            public string EventId { get; }

            public abstract class Response
            {
            }

            public class Success : Response
            {
                public Success(string eventId, string nextApproverId)
                {
                    this.EventId = eventId;
                    this.NextApproverId = nextApproverId;
                }

                public string EventId { get; }

                public string NextApproverId { get; }
            }

            public class Error : Response
            {
                public Error(string eventId, string message)
                {
                    this.EventId = eventId;
                    this.Message = message;
                }

                public string EventId { get; }

                public string Message { get; }
            }
        }
    }
}
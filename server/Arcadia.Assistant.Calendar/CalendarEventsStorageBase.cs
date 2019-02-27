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
        // Ugly, but it is the simplest way to achieve the goal for now
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals/calendar-events-approvals-checker";

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
                            this.OnSuccessfulUpsert(ev);
                            Context.System.EventStream.Publish(new CalendarEventCreated(ev, cmd.UpdatedBy, cmd.Timestamp));

                            this.CompleteEventIfNeeded(ev, cmd.UpdatedBy, cmd.Timestamp);
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.ToString()));
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
                    try
                    {
                        this.ApproveCalendarEvent(msg);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(ex.ToString()));
                    }

                    break;

                case CompleteEventSuccess msg:
                    if (msg.Data != null)
                    {
                        this.UpdateCalendarEvent(msg.Data.OldEvent, msg.Data.CompleteBy, msg.Data.Timestamp, msg.Data.NewEvent, ev =>
                        {
                            Context.System.EventStream.Publish(
                                new CalendarEventChanged(
                                    msg.Data.OldEvent,
                                    msg.Data.CompleteBy,
                                    msg.Data.Timestamp,
                                    msg.Data.NewEvent));
                        });
                    }
                    break;

                case CompleteEventError msg:
                    this.logger.Error(
                        "Error occured on approve event after all user approvals granted " +
                        $"for employee {this.EmployeeId}: {msg.Exception}");
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

                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(calendarEvent, approvals.ToList()));

                this.CompleteEventIfNeeded(calendarEvent, message.ApproverId, message.Timestamp, approvals.ToList());
            });
        }

        private void CompleteEventIfNeeded(
            CalendarEvent @event,
            string approvedBy,
            DateTimeOffset timestamp,
            IEnumerable<Approval> approvals = null)
        {
            this.GetCompleteEventData(@event, approvedBy, timestamp, approvals ?? Enumerable.Empty<Approval>())
                .PipeTo(
                    this.Self,
                    success: result => new CompleteEventSuccess(result),
                    failure: err => new CompleteEventError(err));
        }

        private async Task<CompleteEventData> GetCompleteEventData(
            CalendarEvent @event,
            string completeBy,
            DateTimeOffset timestamp,
            IEnumerable<Approval> approvals = null)
        {
            var nextApproverId = await this.GetNextApproverId(@event, approvals ?? Enumerable.Empty<Approval>());
            if (nextApproverId != null)
            {
                return null;
            }

            var approvedStatus = new CalendarEventStatuses().ApprovedForType(@event.Type);
            var newEvent = new CalendarEvent(
                @event.EventId,
                @event.Type,
                @event.Dates,
                approvedStatus,
                @event.EmployeeId
            );

            return new CompleteEventData(newEvent, @event, completeBy, timestamp);
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
        private void OnSuccessfulUpsert(CalendarEvent calendarEvent)
        {
            this.Sender.Tell(new UpsertCalendarEvent.Success(calendarEvent));
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

        private class CompleteEventData
        {
            public CompleteEventData(CalendarEvent newEvent, CalendarEvent oldEvent, string completeBy, DateTimeOffset timestamp)
            {
                this.NewEvent = newEvent;
                this.OldEvent = oldEvent;
                this.CompleteBy = completeBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent NewEvent { get; }

            public CalendarEvent OldEvent { get; }

            public string CompleteBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        private class CompleteEventSuccess
        {
            public CompleteEventSuccess(CompleteEventData data)
            {
                this.Data = data;
            }

            public CompleteEventData Data { get; }
        }

        private class CompleteEventError
        {
            public CompleteEventError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
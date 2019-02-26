namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public abstract class CalendarEventsStorageBase : UntypedPersistentActor, ILogReceive
    {
        protected delegate void OnSuccessfulUpsertCallback(CalendarEvent changedEvent);

        protected string EmployeeId { get; }

        protected readonly Dictionary<string, CalendarEvent> EventsById = new Dictionary<string, CalendarEvent>();
        protected readonly Dictionary<string, List<Approval>> ApprovalsByEvent = new Dictionary<string, List<Approval>>();

        protected CalendarEventsStorageBase(string employeeId)
        {
            this.EmployeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromApprovers>(this.Self);
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

                case CalendarEventAssignedToApprover msg when this.EventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, msg.ApproverId);
                    break;

                case CalendarEventAssignedToApprover _:
                    break;

                case CalendarEventRemovedFromApprovers msg when this.EventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, null);
                    break;

                case CalendarEventRemovedFromApprovers _:
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
    }
}
namespace Arcadia.Assistant.Calendar.Vacations
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
    using Arcadia.Assistant.Calendar.Events;

    public class PersistentEmployeeVacationsRegistry : UntypedPersistentActor, ILogReceive
    {
        private readonly string employeeId;

        private readonly Dictionary<string, CalendarEvent> eventsById =
            new Dictionary<string, CalendarEvent>();
        private readonly Dictionary<string, List<Approval>> approvalsByEvent =
            new Dictionary<string, List<Approval>>();

        public PersistentEmployeeVacationsRegistry(string employeeId)
        {
            this.employeeId = employeeId;

            this.PersistenceId = $"employee-vacations-{employeeId}";

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromApprovers>(this.Self);
        }

        public override string PersistenceId { get; }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.employeeId, this.eventsById.Values.ToList()));
                    break;

                case GetCalendarEvent msg when !this.eventsById.ContainsKey(msg.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEvent msg:
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.eventsById[msg.EventId]));
                    break;

                case UpsertCalendarEvent msg when msg.Event.EventId == null:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(new ArgumentNullException(nameof(msg.Event.EventId)).Message));
                    break;

                case UpsertCalendarEvent cmd when !this.eventsById.ContainsKey(cmd.Event.EventId):
                    try
                    {
                        if (cmd.Event.Status != this.GetInitialStatus())
                        {
                            throw new Exception($"Event {cmd.Event.EventId}. Initial status must be {this.GetInitialStatus()}");
                        }

                        this.EnsureDatesAreNotIntersected(cmd.Event);

                        this.InsertCalendarEvent(cmd.Event, cmd.UpdatedBy, cmd.Timestamp);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                    }

                    break;

                case UpsertCalendarEvent cmd:
                    try
                    {
                        var oldEvent = this.eventsById[cmd.Event.EventId];

                        if (oldEvent.Status != cmd.Event.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, cmd.Event.Status))
                        {
                            throw new Exception(
                                $"Event {cmd.Event.EventId}. Status transition {oldEvent.Status} -> {cmd.Event.Status} " +
                                "is not allowed for vacation");
                        }

                        this.EnsureDatesAreNotIntersected(cmd.Event);

                        this.UpdateCalendarEvent(oldEvent, cmd.UpdatedBy, cmd.Timestamp, cmd.Event);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                    }

                    break;

                case GetCalendarEventApprovals msg:
                    if (!this.approvalsByEvent.TryGetValue(msg.Event.EventId, out var approvals))
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

                case CalendarEventAssignedToApprover msg when this.eventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, msg.ApproverId);
                    break;

                case CalendarEventRemovedFromApprovers msg when this.eventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, null);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case VacationIsRequested ev:
                    this.OnVacationRequested(ev);
                    break;

                case VacationDatesAreEdited ev:
                    this.OnVacationDatesEdit(ev);
                    break;

                case VacationIsRejected ev:
                    this.OnVacationRejected(ev);
                    break;

                case VacationIsApproved ev:
                    this.OnVacationApproved(ev);
                    break;

                case UserGrantedCalendarEventApproval ev:
                    this.OnSuccessfulApprove(ev);
                    break;

                case VacationIsCancelled ev:
                    this.OnVacationCancel(ev);
                    break;

                case RecoveryCompleted _:
                    foreach (var @event in this.eventsById.Values)
                    {
                        if (@event.IsPending)
                        {
                            Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event));
                        }
                    }

                    break;
            }
        }

        private void InsertCalendarEvent(
            CalendarEvent calendarEvent,
            string createdBy,
            DateTimeOffset timestamp)
        {
            this.Persist(new VacationIsRequested
            {
                EmployeeId = this.employeeId,
                EventId = calendarEvent.EventId,
                StartDate = calendarEvent.Dates.StartDate,
                EndDate = calendarEvent.Dates.EndDate,
                TimeStamp = timestamp,
                UserId = createdBy
            }, ev =>
            {
                this.OnVacationRequested(ev);

                Context.System.EventStream.Publish(new CalendarEventCreated(calendarEvent, createdBy, timestamp));
                this.Sender.Tell(new UpsertCalendarEvent.Success(calendarEvent));
            });
        }

        private void UpdateCalendarEvent(
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp,
            CalendarEvent newEvent)
        {
            if (oldEvent.Dates != newEvent.Dates)
            {
                if (!oldEvent.IsPending)
                {
                    throw new Exception($"Date change is not allowed in status {oldEvent.Status} for {oldEvent.Type}");
                }

                if (this.approvalsByEvent.TryGetValue(oldEvent.EventId, out var approvals) && approvals.Count != 0)
                {
                    throw new Exception($"Date change is not allowed when there is at least one user approval for {oldEvent.Type}");
                }

                this.Persist(new VacationDatesAreEdited
                {
                    EventId = newEvent.EventId,
                    StartDate = newEvent.Dates.StartDate,
                    EndDate = newEvent.Dates.EndDate,
                    TimeStamp = timestamp,
                    UserId = updatedBy
                }, this.OnVacationDatesEdit);
            }

            if (oldEvent.Status != newEvent.Status)
            {
                switch (newEvent.Status)
                {
                    case VacationStatuses.Approved:
                        this.Persist(new VacationIsApproved
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnVacationApproved);
                        break;

                    case VacationStatuses.Cancelled:
                        this.Persist(new VacationIsCancelled
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnVacationCancel);
                        break;

                    case VacationStatuses.Rejected:
                        this.Persist(new VacationIsRejected
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnVacationRejected);
                        break;
                }
            }

            Context.System.EventStream.Publish(new CalendarEventChanged(
                oldEvent,
                updatedBy,
                timestamp,
                newEvent));

            this.Sender.Tell(new UpsertCalendarEvent.Success(newEvent));
        }

        private void ApproveCalendarEvent(ApproveCalendarEvent message)
        {
            var calendarEvent = this.eventsById[message.Event.EventId];
            var approvals = this.approvalsByEvent[message.Event.EventId];

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

            this.Persist(new UserGrantedCalendarEventApproval
            {
                EventId = message.Event.EventId,
                TimeStamp = message.Timestamp,
                UserId = message.ApproverId
            }, ev =>
            {
                this.OnSuccessfulApprove(ev);

                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(calendarEvent, approvals.ToList()));
            });
        }

        protected virtual void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var approvals = this.approvalsByEvent[message.EventId];
            approvals.Add(new Approval(message.TimeStamp, message.UserId));
        }

        private void OnCalendarEventNextApproverReceived(string eventId, string nextApproverId)
        {
            var oldEvent = this.eventsById[eventId];
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

                var approvals = this.approvalsByEvent[eventId];

                var lastApproval = approvals
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefault();

                // If there is no approvals, then employee is Director General and event is updated by himself
                var updatedBy = lastApproval?.ApprovedBy ?? newEvent.EmployeeId;
                var timestamp = lastApproval?.Timestamp ?? DateTimeOffset.Now;

                this.UpdateCalendarEvent(oldEvent, updatedBy, timestamp, newEvent);
            }
        }

        private void OnVacationRequested(VacationIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            var calendarEvent = new CalendarEvent(
                message.EventId,
                CalendarEventTypes.Vacation,
                datesPeriod,
                VacationStatuses.Requested,
                this.employeeId);
            this.eventsById[message.EventId] = calendarEvent;
            this.approvalsByEvent[message.EventId] = new List<Approval>();
        }

        private void OnVacationDatesEdit(VacationDatesAreEdited message)
        {
            if (this.eventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var newDates = new DatesPeriod(message.StartDate, message.EndDate);
                this.eventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    newDates,
                    calendarEvent.Status,
                    calendarEvent.EmployeeId);
            }
        }

        private void OnVacationApproved(VacationIsApproved message)
        {
            if (this.eventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.eventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    calendarEvent.Dates,
                    VacationStatuses.Approved,
                    calendarEvent.EmployeeId);
            }
        }

        private void OnVacationCancel(VacationIsCancelled message)
        {
            if (this.eventsById.ContainsKey(message.EventId))
            {
                this.eventsById.Remove(message.EventId);
            }
        }

        private void OnVacationRejected(VacationIsRejected message)
        {
            if (this.eventsById.ContainsKey(message.EventId))
            {
                this.eventsById.Remove(message.EventId);
            }
        }

        private string GetInitialStatus()
        {
            return VacationStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return VacationStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != VacationStatuses.Cancelled)
                && (oldCalendarEventStatus != VacationStatuses.Rejected)
                && (newCalendarEventStatus != this.GetInitialStatus())
                && (newCalendarEventStatus != VacationStatuses.Approved);
        }

        private void EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var intersectedEvent = this.eventsById.Values
                .Where(ev => ev.EventId != @event.EventId)
                .FirstOrDefault(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            if (intersectedEvent != null)
            {
                throw new Exception($"Event {@event.EventId}. Dates intersect with another vacation with id {intersectedEvent.EventId}");
            }
        }
    }
}
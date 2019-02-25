namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
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

                case GetCalendarEventApprovals msg:
                    if (!this.approvalsByEvent.TryGetValue(msg.Event.EventId, out var approvals))
                    {
                        this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Event with event id {msg.Event.EventId} is not found"));
                    }

                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(approvals));
                    break;

                case InsertVacation msg when msg.Event.EventId == null:
                    this.Sender.Tell(new InsertVacation.Error(new ArgumentNullException(nameof(msg.Event.EventId))));
                    break;

                case InsertVacation cmd:
                    this.InsertVacation(cmd.Event, cmd.CreatedBy, cmd.Timestamp);
                    break;

                case UpdateVacation cmd:
                    this.UpdateVacation(cmd.OldEvent, cmd.UpdatedBy, cmd.Timestamp, cmd.NewEvent);
                    break;

                case ApproveVacation msg:
                    try
                    {
                        this.ApproveVacation(msg);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(ex.Message));
                    }

                    break;

                case CheckDatesAvailability msg:
                    var datesAvailable = this.CheckDatesAvailability(msg.Event);
                    this.Sender.Tell(new CheckDatesAvailability.Success(datesAvailable));
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
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event));
                    }

                    break;
            }
        }

        private void InsertVacation(
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
                this.Sender.Tell(new InsertVacation.Success(calendarEvent, createdBy, timestamp));
            });
        }

        private void UpdateVacation(
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp,
            CalendarEvent newEvent)
        {
            if (oldEvent.Dates != newEvent.Dates)
            {
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

            this.Sender.Tell(new UpdateVacation.Success(newEvent, oldEvent, updatedBy, timestamp));
        }

        private void ApproveVacation(ApproveVacation message)
        {
            if (!this.eventsById.ContainsKey(message.Event.EventId))
            {
                throw new Exception($"Vacation with id {message.Event.EventId} is not found");
            }

            var calendarEvent = this.eventsById[message.Event.EventId];
            var approvals = this.approvalsByEvent[message.Event.EventId];

            if (approvals.Any(a => a.ApprovedBy == message.ApprovedBy))
            {
                this.Sender.Tell(Abstractions.EmployeeVacations.ApproveVacation.Success.Instance);
                return;
            }

            this.Persist(new UserGrantedCalendarEventApproval
            {
                EventId = message.Event.EventId,
                TimeStamp = message.Timestamp,
                UserId = message.ApprovedBy
            }, ev =>
            {
                this.OnSuccessfulApprove(ev);
                this.Sender.Tell(new ApproveVacation.Success(calendarEvent, approvals.ToList(), message.ApprovedBy, message.Timestamp));
            });
        }

        private void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var approvals = this.approvalsByEvent[message.EventId];
            approvals.Add(new Approval(message.TimeStamp, message.UserId));
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

        private bool CheckDatesAvailability(CalendarEvent @event)
        {
            var intersectedEventExists = this.eventsById.Values
                .Where(ev => ev.EventId != @event.EventId)
                .Any(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            return !intersectedEventExists;
        }
    }
}
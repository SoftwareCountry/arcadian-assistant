namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.Vacations.Events;

    public class EmployeeVacationsActor : CalendarEventsStorageBase
    {
        public override string PersistenceId { get; }

        private int vacationsCredit = 28;

        public EmployeeVacationsActor(string employeeId)
            : base(employeeId)
        {
            this.PersistenceId = $"employee-vacations-{this.EmployeeId}";
        }

        public static Props CreateProps(string employeeId)
        {
            return Props.Create(() => new EmployeeVacationsActor(employeeId));
        }

        protected override void InsertCalendarEvent(CalendarEvent calendarEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            var eventId = calendarEvent.EventId;
            var newEvent = new VacationIsRequested()
                {
                    EmployeeId = this.EmployeeId,
                    EventId = eventId,
                    StartDate = calendarEvent.Dates.StartDate,
                    EndDate = calendarEvent.Dates.EndDate,
                    TimeStamp = DateTimeOffset.Now
                };
            this.Persist(newEvent, e =>
                {
                    this.OnVacationRequested(e);
                    onUpsert(this.EventsById[eventId]);
                });
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetVacationsCredit _:
                    this.Sender.Tell(new GetVacationsCredit.Response(this.vacationsCredit));
                    break;

                default:
                    base.OnCommand(message);
                    break;
            }
        }

        protected override void UpdateCalendarEvent(CalendarEvent oldEvent, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            if (oldEvent.Dates != newEvent.Dates)
            {
                var eventToPersist = new VacationDatesAreEdited()
                    {
                        EventId = newEvent.EventId,
                        StartDate = newEvent.Dates.StartDate,
                        EndDate = newEvent.Dates.EndDate,
                        TimeStamp = DateTimeOffset.Now
                    };
                this.Persist(eventToPersist, this.OnVacationDatesEdit);
            }

            if (oldEvent.Status != newEvent.Status)
            {
                switch (newEvent.Status)
                {
                    case VacationStatuses.Approved:
                        this.Persist(new VacationIsApproved()
                            {
                                EventId = newEvent.EventId,
                                TimeStamp = DateTimeOffset.Now
                            }, this.OnVacationApproved);                    
                        break;

                    case VacationStatuses.Cancelled:
                        this.Persist(new VacationIsCancelled()
                            {
                                EventId = newEvent.EventId,
                                TimeStamp = DateTimeOffset.Now
                            }, this.OnVacationCancel);
                        break;

                    case VacationStatuses.Rejected:
                        this.Persist(new VacationIsRejected()
                            {
                                EventId = newEvent.EventId,
                                TimeStamp = DateTimeOffset.Now,
                            }, this.OnVacationRejected);
                        break;
                }
            }

            onUpsert(newEvent);
        }

        protected override string GetInitialStatus()
        {
            return VacationStatuses.Requested;
        }

        protected override bool IsStatusTransitionAllowed(CalendarEvent oldCalendarEvent, CalendarEvent newCalendarEvent)
        {
            return VacationStatuses.All.Contains(newCalendarEvent.Status)
                && (oldCalendarEvent.Status != VacationStatuses.Cancelled)
                && (oldCalendarEvent.Status != VacationStatuses.Rejected)
                && (newCalendarEvent.Status != this.GetInitialStatus());
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

                case VacationIsCancelled ev:
                    this.OnVacationCancel(ev);
                    break;
            }
        }

        private void OnVacationRequested(VacationIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            var calendarEvent = new CalendarEvent(message.EventId, CalendarEventTypes.Vacation, datesPeriod, VacationStatuses.Requested);
            this.EventsById[message.EventId] = calendarEvent;
        }

        private void OnVacationDatesEdit(VacationDatesAreEdited message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var newDates = new DatesPeriod(message.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, newDates, calendarEvent.Status);
            }
        }

        private void OnVacationApproved(VacationIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, VacationStatuses.Approved);
            }
        }

        private void OnVacationCancel(VacationIsCancelled message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
            }
        }

        private void OnVacationRejected(VacationIsRejected message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
            }
        }
    }
}
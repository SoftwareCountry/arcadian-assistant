namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.SickLeave.Events;

    public class EmployeeSickLeaveActor : CalendarEventsStorageBase
    {
        private readonly IActorRef sickLeaveNotifications;

        public EmployeeSickLeaveActor(string employeeId, IActorRef sickLeaveNotifications)
            : base(employeeId)
        {
            this.PersistenceId = $"employee-sickleaves-{this.EmployeeId}";
            this.sickLeaveNotifications = sickLeaveNotifications;
        }

        public override string PersistenceId { get; }

        public static Props CreateProps(string employeeId, IActorRef sickLeaveNotifications)
        {
            return Props.Create(() => new EmployeeSickLeaveActor(employeeId, sickLeaveNotifications));
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case SickLeaveIsRequested ev:
                    this.OnSickLeaveRequest(ev);
                    break;

                case SickLeaveIsCancelled ev:
                    this.OnSickLeaveCancelled(ev);
                    break;

                case SickLeaveIsCompleted ev:
                    this.OnSickLeaveCompleted(ev);
                    break;

                case SickLeaveIsProlonged ev:
                    this.OnSickLeaveProlonged(ev);
                    break;

                case SickLeaveIsRejected ev:
                    this.OnSickLeaveRejected(ev);
                    break;

                case SickLeaveIsApproved ev:
                    this.OnSickleaveApproved(ev);
                    break;
            }
        }

        protected override void InsertCalendarEvent(CalendarEvent calendarEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            var eventId = calendarEvent.EventId;
            var newEvent = new SickLeaveIsRequested()
            {
                EmployeeId = this.EmployeeId,
                EventId = eventId,
                StartDate = calendarEvent.Dates.StartDate,
                EndDate = calendarEvent.Dates.EndDate,
                TimeStamp = DateTimeOffset.Now
            };
            this.Persist(newEvent, e =>
                {
                    this.OnSickLeaveRequest(e);
                    onUpsert(this.EventsById[eventId]);
                });
        }

        protected override void UpdateCalendarEvent(CalendarEvent oldEvent, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            if (oldEvent.Dates.StartDate != newEvent.Dates.StartDate)
            {
                throw new Exception("Start date cannot be changed");
            }

            if (oldEvent.Dates.EndDate != newEvent.Dates.EndDate)
            {
                this.Persist(new SickLeaveIsProlonged()
                {
                    EndDate = newEvent.Dates.EndDate,
                    EventId = newEvent.EventId,
                    TimeStamp = DateTimeOffset.Now
                }, this.OnSickLeaveProlonged);
            }

            if (oldEvent.Status != newEvent.Status)
            {

                switch (newEvent.Status)
                {
                    case SickLeaveStatuses.Cancelled:
                        this.Persist(new SickLeaveIsCancelled()
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now
                        }, this.OnSickLeaveCancelled);
                        break;

                    case SickLeaveStatuses.Completed:
                        this.Persist(new SickLeaveIsCompleted()
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now
                        }, this.OnSickLeaveCompleted);
                        break;

                    case SickLeaveStatuses.Approved:
                        this.Persist(new SickLeaveIsApproved()
                            {
                                EventId = newEvent.EventId,
                                TimeStamp = DateTimeOffset.Now
                            }, this.OnSickleaveApproved);
                        break;

                    case SickLeaveStatuses.Rejected:
                        this.Persist(new SickLeaveIsRejected()
                            {
                                EventId = newEvent.EventId,
                                TimeStamp = DateTimeOffset.Now
                            }, this.OnSickLeaveRejected);
                        break;
                }
            }

            onUpsert(newEvent);
        }

        protected override string GetInitialStatus()
        {
            return SickLeaveStatuses.Requested;
        }

        protected override bool IsStatusTransitionAllowed(CalendarEvent oldCalendarEvent, CalendarEvent newCalendarEvent)
        {
            return SickLeaveStatuses.All.Contains(newCalendarEvent.Status)
                && (oldCalendarEvent.Status != SickLeaveStatuses.Cancelled)
                && (oldCalendarEvent.Status != SickLeaveStatuses.Completed)
                && (newCalendarEvent.Status != this.GetInitialStatus());
        }

        private void OnSickLeaveRequest(SickLeaveIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            this.EventsById[message.EventId] = new CalendarEvent(message.EventId, CalendarEventTypes.Sickleave, datesPeriod, SickLeaveStatuses.Requested, this.EmployeeId);
        }

        private void OnSickLeaveCompleted(SickLeaveIsCompleted message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, SickLeaveStatuses.Completed, this.EmployeeId);
            }
        }

        private void OnSickLeaveProlonged(SickLeaveIsProlonged message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var dates = new DatesPeriod(calendarEvent.Dates.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, dates, calendarEvent.Status, this.EmployeeId);
            }
        }

        private void OnSickLeaveCancelled(SickLeaveIsCancelled message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
            }
        }

        private void OnSickleaveApproved(SickLeaveIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, SickLeaveStatuses.Approved, this.EmployeeId);
                sickLeaveNotifications.Tell(message);
            }
        }

        private void OnSickLeaveRejected(SickLeaveIsRejected message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
            }
        }
    }
}
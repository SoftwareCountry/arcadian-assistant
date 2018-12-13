namespace Arcadia.Assistant.Calendar.WorkHours
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.WorkHours.Events;

    public class EmployeeWorkHoursActor : CalendarEventsStorageBase
    {
        /// <summary>
        /// Positive values means that these days must be worked out.
        /// Negative means that these can be taked as days off
        /// </summary>
        private int hoursCredit = 0;

        public EmployeeWorkHoursActor(string employeeId, IActorRef calendarEventsApprovalsChecker)
            : base(employeeId, calendarEventsApprovalsChecker)
        {
            this.PersistenceId = $"employee-workhours-{this.EmployeeId}";
        }

        public static Props CreateProps(string employeeId, IActorRef calendarEventsApprovalsChecker)
        {
            return Props.Create(() => new EmployeeWorkHoursActor(employeeId, calendarEventsApprovalsChecker));
        }

        public override string PersistenceId { get; }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case WorkHoursChangeIsRequested ev:
                    this.OnChangeRequested(ev);
                    break;

                case WorkHoursDatesAreEdited ev:
                    this.OnDatesEdit(ev);
                    break;

                case WorkHoursChangeIsApproved ev:
                    this.OnChangeApproved(ev);
                    break;

                case WorkHoursChangeIsCancelled ev:
                    this.OnChangeCancelled(ev);
                    break;

                case WorkHoursChangeIsRejected ev:
                    this.OnChangeRejected(ev);
                    break;

                case UserGrantedCalendarEventApproval ev:
                    this.OnSuccessfulApprove(ev);
                    break;

                case RecoveryCompleted _:
                    foreach (var @event in this.EventsById.Values)
                    {
                        if (@event.IsPending)
                        {
                            this.Self.Tell(new AssignCalendarEventNextApprover(@event.EventId));
                        }
                    }
                    break;
            }
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetWorkHoursCredit _:
                    this.Sender.Tell(new GetWorkHoursCredit.Response(this.hoursCredit));
                    break;

                default:
                    base.OnCommand(message);
                    break;
            }
        }

        protected override void InsertCalendarEvent(
            CalendarEvent calendarEvent,
            string createdBy,
            DateTimeOffset timestamp,
            OnSuccessfulUpsertCallback onUpsert)
        {
            if (calendarEvent.Dates.StartDate != calendarEvent.Dates.EndDate)
            {
                throw new Exception("StartDate must be equal to EndDate for work hours change");
            }

            if (calendarEvent.Dates.StartWorkingHour >= calendarEvent.Dates.FinishWorkingHour)
            {
                throw new Exception("Finish working hour must be greater than start working hour");
            }

            var eventId = calendarEvent.EventId;
            var newEvent = new WorkHoursChangeIsRequested
            {
                EmployeeId = this.EmployeeId,
                EventId = eventId,
                Date = calendarEvent.Dates.StartDate,
                StartHour = calendarEvent.Dates.StartWorkingHour,
                EndHour = calendarEvent.Dates.FinishWorkingHour,
                IsDayoff = calendarEvent.Type == CalendarEventTypes.Dayoff,
                TimeStamp = timestamp,
                UserId = createdBy
            };
            this.Persist(newEvent, e =>
                {
                    this.OnChangeRequested(e);
                    onUpsert(this.EventsById[eventId]);
                });
        }

        protected override void UpdateCalendarEvent(CalendarEvent oldEvent, string updatedBy, DateTimeOffset timestamp, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            if (oldEvent.Dates != newEvent.Dates)
            {
                this.Persist(new WorkHoursDatesAreEdited
                {
                    EventId = newEvent.EventId,
                    StartDate = newEvent.Dates.StartDate,
                    EndDate = newEvent.Dates.EndDate,
                    TimeStamp = timestamp,
                    UserId = updatedBy
                }, this.OnDatesEdit);
            }

            if (oldEvent.Status != newEvent.Status)
            {
                switch (newEvent.Status)
                {
                    case WorkHoursChangeStatuses.Approved:
                        this.Persist(new WorkHoursChangeIsApproved
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now,
                            UserId = updatedBy
                        }, this.OnChangeApproved);
                        break;

                    case WorkHoursChangeStatuses.Cancelled:
                        this.Persist(new WorkHoursChangeIsCancelled
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnChangeCancelled);
                        break;

                    case WorkHoursChangeStatuses.Rejected:
                        this.Persist(new WorkHoursChangeIsRejected
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnChangeRejected);
                        break;
                }
            }

            onUpsert(newEvent);
        }

        protected override string GetInitialStatus()
        {
            return WorkHoursChangeStatuses.Requested;
        }

        protected override bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return WorkHoursChangeStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != WorkHoursChangeStatuses.Cancelled)
                && (oldCalendarEventStatus != WorkHoursChangeStatuses.Rejected)
                && (newCalendarEventStatus != this.GetInitialStatus())
                && (newCalendarEventStatus != WorkHoursChangeStatuses.Approved);
        }

        protected override bool IsDatesChangedAllowed(CalendarEvent oldEvent, CalendarEvent newEvent)
        {
            var approvals = this.ApprovalsByEvent[oldEvent.EventId];
            return oldEvent.IsPending && approvals.Count == 0;
        }

        private void OnChangeRequested(WorkHoursChangeIsRequested message)
        {
            var eventType = message.IsDayoff ? CalendarEventTypes.Dayoff : CalendarEventTypes.Workout;

            var datesPeriod = new DatesPeriod(message.Date, message.Date, message.StartHour, message.EndHour);
            var calendarEvent = new CalendarEvent(
                message.EventId,
                eventType,
                datesPeriod,
                WorkHoursChangeStatuses.Requested,
                this.EmployeeId);
            this.EventsById[message.EventId] = calendarEvent;
            this.ApprovalsByEvent[message.EventId] = new List<Approval>();
        }

        private void OnDatesEdit(WorkHoursDatesAreEdited message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var newDates = new DatesPeriod(message.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    newDates,
                    calendarEvent.Status,
                    calendarEvent.EmployeeId);
            }
        }

        private void OnChangeCancelled(WorkHoursChangeIsCancelled message)
        {
            this.RemoveEvent(message.EventId);
        }

        private void OnChangeRejected(WorkHoursChangeIsRejected message)
        {
            this.RemoveEvent(message.EventId);
        }

        private void OnChangeApproved(WorkHoursChangeIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                //Make changes to the counter
                this.ChangeCounter(calendarEvent.Dates.StartWorkingHour, calendarEvent.Dates.FinishWorkingHour, this.IsCreditingType(calendarEvent.Type));
                this.EventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    calendarEvent.Dates,
                    WorkHoursChangeStatuses.Approved,
                    this.EmployeeId);
            }
        }

        private void RemoveEvent(string eventId)
        {
            if (this.EventsById.TryGetValue(eventId, out var calendarEvent))
            {
                //If it were approved before, revert changes to counter
                if (calendarEvent.Status == WorkHoursChangeStatuses.Approved)
                {
                    this.ChangeCounter(calendarEvent.Dates.StartWorkingHour, calendarEvent.Dates.FinishWorkingHour, !this.IsCreditingType(calendarEvent.Type));
                }

                this.EventsById.Remove(eventId);
            }
        }

        private bool IsCreditingType(string eventType)
        {
            return eventType == CalendarEventTypes.Dayoff;
        }

        private void ChangeCounter(int startWorkingHour, int finishWorkingHour, bool isCredit)
        {
            var diff = finishWorkingHour - startWorkingHour;
            if (isCredit)
            {
                this.hoursCredit += diff;
            }
            else
            {
                this.hoursCredit -= diff;
            }
        }
    }
}
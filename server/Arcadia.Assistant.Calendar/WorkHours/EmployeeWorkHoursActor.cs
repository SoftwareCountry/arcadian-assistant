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

        private readonly HashSet<string> eventsToProcessApproversAfterRecover = new HashSet<string>();

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
                    foreach (var eventId in this.eventsToProcessApproversAfterRecover)
                    {
                        this.Self.Tell(new AssignCalendarEventNextApprover(eventId));
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

        protected override void InsertCalendarEvent(CalendarEvent calendarEvent, OnSuccessfulUpsertCallback onUpsert)
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
            var newEvent = new WorkHoursChangeIsRequested()
            {
                EmployeeId = this.EmployeeId,
                EventId = eventId,
                Date = calendarEvent.Dates.StartDate,
                StartHour = calendarEvent.Dates.StartWorkingHour,
                EndHour = calendarEvent.Dates.FinishWorkingHour,
                IsDayoff = calendarEvent.Type == CalendarEventTypes.Dayoff,
                TimeStamp = DateTimeOffset.Now
            };
            this.Persist(newEvent, e =>
                {
                    this.OnChangeRequested(e);
                    onUpsert(this.EventsById[eventId]);
                });
        }

        protected override void UpdateCalendarEvent(CalendarEvent oldEvent, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert)
        {
            if (oldEvent.Dates != newEvent.Dates)
            {
                throw new Exception("Dates change is not supported for work hours");
            }

            if (oldEvent.Status != newEvent.Status)
            {
                switch (newEvent.Status)
                {
                    case WorkHoursChangeStatuses.Approved:
                        this.Persist(new WorkHoursChangeIsApproved()
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now
                        }, this.OnChangeApproved);
                        break;

                    case WorkHoursChangeStatuses.Cancelled:
                        this.Persist(new WorkHoursChangeIsCancelled()
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now
                        }, this.OnChangeCancelled);
                        break;

                    case WorkHoursChangeStatuses.Rejected:
                        this.Persist(new WorkHoursChangeIsRejected()
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = DateTimeOffset.Now
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

        protected override void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var approvals = this.ApprovalsByEvent[message.EventId];
            approvals.Add(message.ApproverId);
        }

        private void OnChangeRequested(WorkHoursChangeIsRequested message)
        {
            var eventType = message.IsDayoff ? CalendarEventTypes.Dayoff : CalendarEventTypes.Workout;

            var datesPeriod = new DatesPeriod(message.Date, message.Date, message.StartHour, message.EndHour);
            var calendarEvent = new CalendarEvent(message.EventId, eventType, datesPeriod, WorkHoursChangeStatuses.Requested, this.EmployeeId);
            this.EventsById[message.EventId] = calendarEvent;
            this.ApprovalsByEvent[message.EventId] = new List<string>();
            this.eventsToProcessApproversAfterRecover.Add(message.EventId);
        }

        private void OnChangeCancelled(WorkHoursChangeIsCancelled message)
        {
            this.RemoveEvent(message.EventId);
            this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
        }

        private void OnChangeRejected(WorkHoursChangeIsRejected message)
        {
            this.RemoveEvent(message.EventId);
            this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
        }

        private void OnChangeApproved(WorkHoursChangeIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                //Make changes to the counter
                this.ChangeCounter(calendarEvent.Dates.StartWorkingHour, calendarEvent.Dates.FinishWorkingHour, this.IsCreditingType(calendarEvent.Type));
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, WorkHoursChangeStatuses.Approved, this.EmployeeId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
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
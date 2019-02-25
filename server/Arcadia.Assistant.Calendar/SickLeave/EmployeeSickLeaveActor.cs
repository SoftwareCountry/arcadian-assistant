namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Events;
    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeSickLeaveActor : CalendarEventsStorageBase
    {
        public EmployeeSickLeaveActor(EmployeeMetadata employee)
            : base(employee.EmployeeId)
        {
            this.PersistenceId = $"employee-sickleaves-{this.EmployeeId}";
        }

        public override string PersistenceId { get; }

        public static Props CreateProps(EmployeeMetadata employee)
        {
            return Props.Create(() => new EmployeeSickLeaveActor(employee));
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

                case UserGrantedCalendarEventApproval ev:
                    this.OnSuccessfulApprove(ev);
                    break;

                case RecoveryCompleted _:
                    foreach (var @event in this.EventsById.Values)
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event));
                    }
                    break;
            }
        }

        protected override void InsertCalendarEvent(
            CalendarEvent calendarEvent,
            string createdBy,
            DateTimeOffset timestamp,
            OnSuccessfulUpsertCallback onUpsert)
        {
            var eventId = calendarEvent.EventId;
            var newEvent = new SickLeaveIsRequested()
            {
                EmployeeId = this.EmployeeId,
                EventId = eventId,
                StartDate = calendarEvent.Dates.StartDate,
                EndDate = calendarEvent.Dates.EndDate,
                TimeStamp = timestamp,
                UserId = createdBy
            };
            this.Persist(newEvent, e =>
            {
                this.OnSickLeaveRequest(e);
                onUpsert(this.EventsById[eventId]);
            });
        }

        protected override void UpdateCalendarEvent(
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp,
            CalendarEvent newEvent,
            OnSuccessfulUpsertCallback onUpsert)
        {
            if (oldEvent.Dates.StartDate != newEvent.Dates.StartDate)
            {
                throw new Exception("Start date cannot be changed");
            }

            if (oldEvent.Dates.EndDate != newEvent.Dates.EndDate)
            {
                this.Persist(new SickLeaveIsProlonged
                {
                    EndDate = newEvent.Dates.EndDate,
                    EventId = newEvent.EventId,
                    TimeStamp = timestamp,
                    UserId = updatedBy
                }, this.OnSickLeaveProlonged);
            }

            if (oldEvent.Status != newEvent.Status)
            {
                switch (newEvent.Status)
                {
                    case SickLeaveStatuses.Cancelled:
                        this.Persist(new SickLeaveIsCancelled
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnSickLeaveCancelled);
                        break;

                    case SickLeaveStatuses.Completed:
                        this.Persist(new SickLeaveIsCompleted
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnSickLeaveCompleted);
                        break;

                    case SickLeaveStatuses.Approved:
                        this.Persist(new SickLeaveIsApproved
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
                        }, this.OnSickleaveApproved);
                        break;

                    case SickLeaveStatuses.Rejected:
                        this.Persist(new SickLeaveIsRejected
                        {
                            EventId = newEvent.EventId,
                            TimeStamp = timestamp,
                            UserId = updatedBy
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

        protected override bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return SickLeaveStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != SickLeaveStatuses.Cancelled)
                && (oldCalendarEventStatus != SickLeaveStatuses.Completed)
                && (newCalendarEventStatus != this.GetInitialStatus())
                && (newCalendarEventStatus != SickLeaveStatuses.Approved);
        }

        private void OnSickLeaveRequest(SickLeaveIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            this.EventsById[message.EventId] = new CalendarEvent(
                message.EventId,
                CalendarEventTypes.Sickleave,
                datesPeriod,
                SickLeaveStatuses.Requested,
                this.EmployeeId);
            this.ApprovalsByEvent[message.EventId] = new List<Approval>();
        }

        private void OnSickLeaveCompleted(SickLeaveIsCompleted message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    calendarEvent.Dates,
                    SickLeaveStatuses.Completed,
                    this.EmployeeId);
            }
        }

        private void OnSickLeaveProlonged(SickLeaveIsProlonged message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var dates = new DatesPeriod(calendarEvent.Dates.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    dates,
                    calendarEvent.Status,
                    this.EmployeeId);
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
                this.EventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    calendarEvent.Dates,
                    SickLeaveStatuses.Approved,
                    this.EmployeeId);
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
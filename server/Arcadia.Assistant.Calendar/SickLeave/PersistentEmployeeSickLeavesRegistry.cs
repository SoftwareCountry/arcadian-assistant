namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.Events;

    public class PersistentEmployeeSickLeavesRegistry : UntypedPersistentActor, ILogReceive
    {
        private readonly string employeeId;

        private readonly Dictionary<string, CalendarEvent> eventsById =
            new Dictionary<string, CalendarEvent>();

        public PersistentEmployeeSickLeavesRegistry(string employeeId)
        {
            this.employeeId = employeeId;

            this.PersistenceId = $"employee-sick-leaves-{employeeId}";
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

                case InsertSickLeave msg when msg.Event.EventId == null:
                    this.Sender.Tell(new InsertSickLeave.Error(new ArgumentNullException(nameof(msg.Event.EventId))));
                    break;

                case InsertSickLeave cmd:
                    this.InsertSickLeave(cmd.Event, cmd.CreatedBy, cmd.Timestamp);
                    break;

                case UpdateSickLeave cmd:
                    this.UpdateSickLeave(cmd.OldEvent, cmd.UpdatedBy, cmd.Timestamp, cmd.NewEvent);
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
                case SickLeaveIsRequested ev:
                    this.OnSickLeaveRequested(ev);
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

                case RecoveryCompleted _:
                    foreach (var @event in this.eventsById.Values)
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event));
                    }

                    break;
            }
        }

        private void InsertSickLeave(
            CalendarEvent calendarEvent,
            string createdBy,
            DateTimeOffset timestamp)
        {
            this.Persist(new SickLeaveIsRequested
            {
                EmployeeId = this.employeeId,
                EventId = calendarEvent.EventId,
                StartDate = calendarEvent.Dates.StartDate,
                EndDate = calendarEvent.Dates.EndDate,
                TimeStamp = timestamp,
                UserId = createdBy
            }, ev =>
            {
                this.OnSickLeaveRequested(ev);
                this.Sender.Tell(new InsertSickLeave.Success(calendarEvent, createdBy, timestamp));
            });
        }

        private void UpdateSickLeave(
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp,
            CalendarEvent newEvent)
        {
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
                }
            }

            this.Sender.Tell(new UpdateSickLeave.Success(newEvent, oldEvent, updatedBy, timestamp));
        }

        private void OnSickLeaveRequested(SickLeaveIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);

            this.eventsById[message.EventId] = new CalendarEvent(
                message.EventId,
                CalendarEventTypes.Sickleave,
                datesPeriod,
                SickLeaveStatuses.Requested,
                this.employeeId);
        }

        private void OnSickLeaveCompleted(SickLeaveIsCompleted message)
        {
            if (this.eventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.eventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    calendarEvent.Dates,
                    SickLeaveStatuses.Completed,
                    this.employeeId);
            }
        }

        private void OnSickLeaveProlonged(SickLeaveIsProlonged message)
        {
            if (this.eventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var dates = new DatesPeriod(calendarEvent.Dates.StartDate, message.EndDate);

                this.eventsById[message.EventId] = new CalendarEvent(
                    message.EventId,
                    calendarEvent.Type,
                    dates,
                    calendarEvent.Status,
                    this.employeeId);
            }
        }

        private void OnSickLeaveCancelled(SickLeaveIsCancelled message)
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
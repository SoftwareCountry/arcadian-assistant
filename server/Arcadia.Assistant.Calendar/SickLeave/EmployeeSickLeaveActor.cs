namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.SickLeave.Events;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeSickLeaveActor : CalendarEventsStorageBase
    {
        private EmployeeMetadata employee;
        private readonly HashSet<string> eventsToProcessApproversAfterRecover = new HashSet<string>();

        public EmployeeSickLeaveActor(EmployeeMetadata employee, IActorRef calendarEventsApprovalsChecker)
            : base(employee.EmployeeId, calendarEventsApprovalsChecker)
        {
            this.PersistenceId = $"employee-sickleaves-{this.EmployeeId}";
            this.employee = employee;

            Context.System.EventStream.Subscribe<EmployeeMetadataUpdatedEventBusMessage>(this.Self);
        }

        public override string PersistenceId { get; }

        public static Props CreateProps(EmployeeMetadata employee, IActorRef calendarEventsApprovalsChecker)
        {
            return Props.Create(() => new EmployeeSickLeaveActor(employee, calendarEventsApprovalsChecker));
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case EmployeeMetadataUpdatedEventBusMessage msg:
                    this.employee = msg.EmployeeMetadata;
                    break;

                default:
                    base.OnCommand(message);
                    break;
            }
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
                    foreach (var eventId in this.eventsToProcessApproversAfterRecover)
                    {
                        this.Self.Tell(new ProcessCalendarEventApprovalsMessage(eventId));
                    }
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
                        }, e =>
                        {
                            this.OnSickleaveApproved(e);
                            this.SendSickLeaveApprovedMessage(e);
                        });
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

        protected override bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return SickLeaveStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != SickLeaveStatuses.Cancelled)
                && (oldCalendarEventStatus != SickLeaveStatuses.Completed)
                && (newCalendarEventStatus != this.GetInitialStatus())
                && (newCalendarEventStatus != SickLeaveStatuses.Approved);
        }

        protected override void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var approvals = this.ApprovalsByEvent[message.EventId];
            approvals.Add(message.ApproverId);
        }

        private void OnSickLeaveRequest(SickLeaveIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            this.EventsById[message.EventId] = new CalendarEvent(message.EventId, CalendarEventTypes.Sickleave, datesPeriod, SickLeaveStatuses.Requested, this.EmployeeId);
            this.ApprovalsByEvent[message.EventId] = new List<string>();
            this.eventsToProcessApproversAfterRecover.Add(message.EventId);
        }

        private void OnSickLeaveCompleted(SickLeaveIsCompleted message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, SickLeaveStatuses.Completed, this.EmployeeId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }

        private void OnSickLeaveProlonged(SickLeaveIsProlonged message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var dates = new DatesPeriod(calendarEvent.Dates.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, dates, calendarEvent.Status, this.EmployeeId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }

        private void OnSickLeaveCancelled(SickLeaveIsCancelled message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }

        private void OnSickleaveApproved(SickLeaveIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, SickLeaveStatuses.Approved, this.EmployeeId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }

        private void SendSickLeaveApprovedMessage(SickLeaveIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var notificationPayload = new SendEmailSickLeaveActor.SickLeaveNotification(
                    this.employee.Name,
                    calendarEvent.Dates.StartDate);
                Context.System.EventStream.Publish(new NotificationEventBusMessage(notificationPayload));
            }
        }

        private void OnSickLeaveRejected(SickLeaveIsRejected message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }
    }
}
﻿namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.Vacations.Events;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeVacationsActor : CalendarEventsStorageBase
    {
        private readonly IActorRef employeeFeed;

        private readonly IActorRef vacationsRegistry;

        public override string PersistenceId { get; }

        private readonly HashSet<string> eventsToProcessApproversAfterRecover = new HashSet<string>();

        //private int vacationsCredit = 28;

        public EmployeeVacationsActor(string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsRegistry,
            IActorRef calendarEventsApprovalsChecker
        ) : base(employeeId, calendarEventsApprovalsChecker)
        {
            this.employeeFeed = employeeFeed;
            this.vacationsRegistry = vacationsRegistry;
            this.PersistenceId = $"employee-vacations-{this.EmployeeId}";
        }

        public static Props CreateProps(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsRegistry,
            IActorRef calendarEventsApprovalsChecker)
        {
            return Props.Create(() => new EmployeeVacationsActor(
                employeeId,
                employeeFeed,
                vacationsRegistry,
                calendarEventsApprovalsChecker)
            );
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
                    this.vacationsRegistry
                        .Ask<VacationsRegistry.GetVacationInfo.Response>(new VacationsRegistry.GetVacationInfo(this.EmployeeId))
                        .ContinueWith(x => new GetVacationsCredit.Response(x.Result.VacationsCredit))
                        .PipeTo(this.Sender);
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

        protected override string GetApprovedStatus()
        {
            return VacationStatuses.Approved;
        }

        protected override bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return VacationStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != VacationStatuses.Cancelled)
                && (oldCalendarEventStatus != VacationStatuses.Rejected)
                && (newCalendarEventStatus != this.GetInitialStatus());
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
                    foreach (var eventId in this.eventsToProcessApproversAfterRecover)
                    {
                        this.Self.Tell(new ProcessCalendarEventApprovalsMessage(eventId));
                    }
                    break;
            }
        }

        protected override void OnSuccessfulApprove(UserGrantedCalendarEventApproval message)
        {
            var calendarEvent = this.EventsById[message.EventId];
            var approvals = this.ApprovalsByEvent[message.EventId];

            approvals.Add(message.ApproverId);

            var text = $"Vacation from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()} got one new approval";
            var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
            this.employeeFeed.Tell(new PostMessage(msg));
        }

        private void OnVacationRequested(VacationIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            var calendarEvent = new CalendarEvent(message.EventId, CalendarEventTypes.Vacation, datesPeriod, VacationStatuses.Requested, this.EmployeeId);
            this.EventsById[message.EventId] = calendarEvent;
            this.ApprovalsByEvent[message.EventId] = new List<string>();
            this.eventsToProcessApproversAfterRecover.Add(message.EventId);
        }

        private void OnVacationDatesEdit(VacationDatesAreEdited message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                var newDates = new DatesPeriod(message.StartDate, message.EndDate);
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, newDates, calendarEvent.Status, this.EmployeeId);
            }
        }

        private void OnVacationApproved(VacationIsApproved message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById[message.EventId] = new CalendarEvent(message.EventId, calendarEvent.Type, calendarEvent.Dates, VacationStatuses.Approved, this.EmployeeId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);

                var text = $"Got vacation approved from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()}";
                var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
                this.employeeFeed.Tell(new PostMessage(msg));
            }
        }

        private void OnVacationCancel(VacationIsCancelled message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById.Remove(message.EventId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);

                var text = $"Got vacation canceled ({calendarEvent.Dates.StartDate.ToLongDateString()} - {calendarEvent.Dates.EndDate.ToLongDateString()})";
                var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
                this.employeeFeed.Tell(new PostMessage(msg));
            }
        }

        private void OnVacationRejected(VacationIsRejected message)
        {
            if (this.EventsById.ContainsKey(message.EventId))
            {
                this.EventsById.Remove(message.EventId);
                this.eventsToProcessApproversAfterRecover.Remove(message.EventId);
            }
        }
    }
}
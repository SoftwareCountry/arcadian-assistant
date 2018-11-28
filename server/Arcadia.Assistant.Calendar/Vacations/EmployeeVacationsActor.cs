﻿namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.Vacations.Events;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeVacationsActor : CalendarEventsStorageBase
    {
        private readonly IActorRef employeeFeed;

        private readonly IActorRef vacationsRegistry;
        private readonly IActorRef vacationApprovalsChecker;
        private readonly TimeSpan timeoutSetting;

        public override string PersistenceId { get; }

        private readonly Dictionary<string, List<string>> approvalsByEvent = new Dictionary<string, List<string>>();

        //private int vacationsCredit = 28;

        public EmployeeVacationsActor(string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsRegistry,
            IActorRef vacationApprovalsChecker,
            TimeSpan timeoutSetting
        ) : base(employeeId)
        {
            this.employeeFeed = employeeFeed;
            this.vacationsRegistry = vacationsRegistry;
            this.vacationApprovalsChecker = vacationApprovalsChecker;
            this.timeoutSetting = timeoutSetting;
            this.PersistenceId = $"employee-vacations-{this.EmployeeId}";
        }

        public static Props CreateProps(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsRegistry,
            IActorRef vacationApprovalsChecker,
            TimeSpan timeoutSetting)
        {
            return Props.Create(() => new EmployeeVacationsActor(
                employeeId,
                employeeFeed,
                vacationsRegistry,
                vacationApprovalsChecker,
                timeoutSetting));
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
                this.Self.Tell(new ProcessVacationApprovalsMessage(eventId));
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

                case VacationApproveMessage msg:
                    var @event = new VacationIsApprovedOnce
                    {
                        EventId = msg.EventId,
                        TimeStamp = DateTimeOffset.Now,
                        UserId = msg.ApproverId
                    };

                    this.Persist(@event, ev =>
                    {
                        this.OnVacationApprovedOnce(ev);
                        this.Self.Tell(new ProcessVacationApprovalsMessage(msg.EventId));
                        this.Sender.Tell(VacationApproveMessage.Response.Instance);
                    });
                    break;

                case ProcessVacationApprovalsMessage msg:
                    if (!this.approvalsByEvent.TryGetValue(msg.EventId, out var approvals))
                    {
                        approvals = new List<string>();
                    }

                    var getNextApproverMessage = new GetNextVacationRequestApprover(this.EmployeeId, approvals);

                    this.vacationApprovalsChecker
                        .Ask<GetNextVacationRequestApprover.Response>(getNextApproverMessage)
                        .ContinueWith(task => new ProcessVacationApprovalsMessage.Response(msg.EventId, task.Result.NextApproverEmployeeId))
                        .PipeTo(this.Self);
                    break;

                case ProcessVacationApprovalsMessage.Response msg:
                    if (msg.NextApproverId == null)
                    {
                        var oldEvent = this.EventsById[msg.EventId];

                        var newEvent = new CalendarEvent(
                            oldEvent.EventId,
                            oldEvent.Type,
                            oldEvent.Dates,
                            VacationStatuses.Approved,
                            oldEvent.EmployeeId
                        );

                        this.UpdateCalendarEvent(oldEvent, newEvent, ev => { });
                    }
                    else
                    {
                        // It should publish message to Event Bus that new approver is required
                    }

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

                case VacationIsApprovedOnce ev:
                    this.OnVacationApprovedOnce(ev);
                    break;

                case VacationIsCancelled ev:
                    this.OnVacationCancel(ev);
                    break;
            }
        }

        private void OnVacationRequested(VacationIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            var calendarEvent = new CalendarEvent(message.EventId, CalendarEventTypes.Vacation, datesPeriod, VacationStatuses.Requested, this.EmployeeId);
            this.EventsById[message.EventId] = calendarEvent;
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

                var text = $"Got vacation approved from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()}";
                var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
                this.employeeFeed.Tell(new PostMessage(msg));
            }
        }

        private void OnVacationApprovedOnce(VacationIsApprovedOnce message)
        {
            var calendarEvent = this.EventsById[message.EventId];

            if (!this.approvalsByEvent.TryGetValue(message.EventId, out var approvals))
            {
                approvals = new List<string>();
                this.approvalsByEvent[message.EventId] = approvals;
            }

            approvals.Add(message.UserId);

            var text = $"Vacation from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()} got one new approval";
            var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
            this.employeeFeed.Tell(new PostMessage(msg));
        }

        private void OnVacationCancel(VacationIsCancelled message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById.Remove(message.EventId);

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
            }
        }
    }
}
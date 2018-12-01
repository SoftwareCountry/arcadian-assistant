namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Calendar.Vacations.Events;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeVacationsActor : CalendarEventsStorageBase
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef employeeFeed;

        private readonly IActorRef vacationsRegistry;
        private readonly IActorRef vacationApprovalsChecker;
        private readonly TimeSpan timeoutSetting;

        public override string PersistenceId { get; }

        private readonly Dictionary<string, List<string>> approvalsByEvent = new Dictionary<string, List<string>>();
        private readonly List<string> eventsToCheckApprovers = new List<string>();

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
                        .Ask<VacationsRegistry.GetVacationInfo.Response>(
                            new VacationsRegistry.GetVacationInfo(this.EmployeeId),
                            this.timeoutSetting)
                        .ContinueWith(x => new GetVacationsCredit.Response(x.Result.VacationsCredit))
                        .PipeTo(this.Sender);
                    break;

                case ApproveVacation msg:
                    this.ApproveVacation(msg);
                    break;

                case ProcessVacationApprovalsMessage msg:
                    this.vacationApprovalsChecker
                        .Ask<GetNextVacationRequestApprover.Response>(
                            new GetNextVacationRequestApprover(this.EmployeeId, this.approvalsByEvent[msg.EventId]),
                            this.timeoutSetting)
                        .ContinueWith<ProcessVacationApprovalsMessage.Response>(task =>
                        {
                            if (task.Result is GetNextVacationRequestApprover.ErrorResponse err)
                            {
                                return new ProcessVacationApprovalsMessage.ErrorResponse(msg.EventId, err.Message);
                            }

                            var resp = (GetNextVacationRequestApprover.SuccessResponse)task.Result;
                            return new ProcessVacationApprovalsMessage.SuccessResponse(msg.EventId, resp.NextApproverEmployeeId);
                        })
                        .PipeTo(this.Self);
                    break;

                case ProcessVacationApprovalsMessage.SuccessResponse msg:
                    this.ProcessVacationApprovals(msg);
                    break;

                case ProcessVacationApprovalsMessage.ErrorResponse msg:
                    this.logger.Warning($"Failed to get next approver for the event {msg.EventId}");
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

                case UserGrantedVacationApproval ev:
                    this.OnUserGrantedVacationApproval(ev);
                    break;

                case VacationIsCancelled ev:
                    this.OnVacationCancel(ev);
                    break;

                case RecoveryCompleted _:
                    foreach (var eventId in this.eventsToCheckApprovers)
                    {
                        this.Self.Tell(new ProcessVacationApprovalsMessage(eventId));
                    }
                    break;
            }
        }

        private void ApproveVacation(ApproveVacation message)
        {
            var calendarEvent = this.EventsById[message.EventId];
            var approvals = this.approvalsByEvent[message.EventId];
            if (!this.IsStatusTransitionAllowed(calendarEvent.Status, VacationStatuses.Approved))
            {
                var errorMessage = $"Event {message.EventId}. Status transition {calendarEvent.Status} -> {VacationStatuses.Approved} is not allowed for {calendarEvent.Type}";
                this.Sender.Tell(new ApproveVacation.BadRequestResponse(errorMessage));
                return;
            }

            if (approvals.Contains(message.ApproverId) || calendarEvent.Status == VacationStatuses.Approved)
            {
                this.Sender.Tell(Abstractions.Messages.ApproveVacation.SuccessResponse.Instance);
                return;
            }

            var @event = new UserGrantedVacationApproval
            {
                EventId = message.EventId,
                TimeStamp = DateTimeOffset.Now,
                ApproverId = message.ApproverId
            };

            this.Persist(@event, ev =>
            {
                this.OnUserGrantedVacationApproval(ev);
                this.Self.Tell(new ProcessVacationApprovalsMessage(message.EventId));
                this.Sender.Tell(Abstractions.Messages.ApproveVacation.SuccessResponse.Instance);
            });
        }

        private void ProcessVacationApprovals(ProcessVacationApprovalsMessage.SuccessResponse successResponse)
        {
            var oldEvent = this.EventsById[successResponse.EventId];

            if (successResponse.NextApproverId == null)
            {
                var newEvent = new CalendarEvent(
                    oldEvent.EventId,
                    oldEvent.Type,
                    oldEvent.Dates,
                    VacationStatuses.Approved,
                    oldEvent.EmployeeId
                );

                this.UpdateCalendarEvent(oldEvent, newEvent, ev => { });

                Context.System.EventStream.Publish(
                    new CalendarEventApproverEventBusMessage(oldEvent, null));
            }
            else
            {
                Context.System.EventStream.Publish(
                    new CalendarEventApproverEventBusMessage(oldEvent, successResponse.NextApproverId));
            }
        }

        private void OnVacationRequested(VacationIsRequested message)
        {
            var datesPeriod = new DatesPeriod(message.StartDate, message.EndDate);
            var calendarEvent = new CalendarEvent(message.EventId, CalendarEventTypes.Vacation, datesPeriod, VacationStatuses.Requested, this.EmployeeId);
            this.EventsById[message.EventId] = calendarEvent;
            this.approvalsByEvent[message.EventId] = new List<string>();
            this.eventsToCheckApprovers.Add(message.EventId);
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
                this.eventsToCheckApprovers.Remove(message.EventId);

                var text = $"Got vacation approved from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()}";
                var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
                this.employeeFeed.Tell(new PostMessage(msg));
            }
        }

        private void OnUserGrantedVacationApproval(UserGrantedVacationApproval message)
        {
            var calendarEvent = this.EventsById[message.EventId];
            var approvals = this.approvalsByEvent[message.EventId];

            approvals.Add(message.ApproverId);

            var text = $"Vacation from {calendarEvent.Dates.StartDate.ToLongDateString()} to {calendarEvent.Dates.EndDate.ToLongDateString()} got one new approval";
            var msg = new Message(Guid.NewGuid().ToString(), this.EmployeeId, "Vacation", text, message.TimeStamp.Date);
            this.employeeFeed.Tell(new PostMessage(msg));
        }

        private void OnVacationCancel(VacationIsCancelled message)
        {
            if (this.EventsById.TryGetValue(message.EventId, out var calendarEvent))
            {
                this.EventsById.Remove(message.EventId);
                this.eventsToCheckApprovers.Remove(message.EventId);

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
                this.eventsToCheckApprovers.Remove(message.EventId);
            }
        }

        private class ProcessVacationApprovalsMessage
        {
            public ProcessVacationApprovalsMessage(string eventId)
            {
                this.EventId = eventId;
            }

            public string EventId { get; }

            public abstract class Response
            {
            }

            public class SuccessResponse : Response
            {
                public SuccessResponse(string eventId, string nextApproverId)
                {
                    this.EventId = eventId;
                    this.NextApproverId = nextApproverId;
                }

                public string EventId { get; }

                public string NextApproverId { get; }
            }

            public class ErrorResponse : Response
            {
                public ErrorResponse(string eventId, string message)
                {
                    this.EventId = eventId;
                    this.Message = message;
                }

                public string EventId { get; }

                public string Message { get; }
            }
        }
    }
}
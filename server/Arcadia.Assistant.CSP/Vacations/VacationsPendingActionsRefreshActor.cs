namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class VacationsPendingActionsRefreshActor : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly ActorSelection calendarEventsApprovalsChecker;
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly Dictionary<string, CalendarEvent> eventsById = new Dictionary<string, CalendarEvent>();
        private readonly Dictionary<string, List<string>> approversByEvent = new Dictionary<string, List<string>>();

        public VacationsPendingActionsRefreshActor()
        {
            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            Context.System.EventStream.Subscribe<CalendarEventAddedToPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemoved>(this.Self);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1),
                this.Self,
                RefreshPendingActions.Instance,
                this.Self);
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<VacationsPendingActionsRefreshActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshPendingActions _:
                    foreach (var @event in this.eventsById.Values)
                    {
                        this.GetNextApproverId(@event, this.approversByEvent[@event.EventId])
                            .PipeTo(
                                this.Self,
                                success: result => new RefreshEventPendingActionsSuccess(@event, result),
                                failure: error => new RefreshEventPendingActionsFailure(@event, error));
                    }

                    break;

                case RefreshEventPendingActionsSuccess msg:
                    if (msg.NextApprover != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }

                    break;

                case RefreshEventPendingActionsFailure msg:
                    this.logger.Warning($"An error occured while updating pending actions for event {msg.Event.EventId}. {msg.Exception}");
                    break;

                case CalendarEventAddedToPendingActions msg:
                    this.AddApprover(msg.Event, msg.ApproverId);
                    break;

                case CalendarEventRemovedFromPendingActions msg:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                case CalendarEventChanged msg when !msg.NewEvent.IsPending:
                    this.RemoveEvent(msg.NewEvent.EventId);
                    break;

                case CalendarEventChanged msg:
                    this.eventsById[msg.NewEvent.EventId] = msg.NewEvent;
                    break;

                case CalendarEventApprovalsChanged msg when !msg.Event.IsPending:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                case CalendarEventApprovalsChanged _:
                    break;

                case CalendarEventRemoved msg:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void AddApprover(CalendarEvent @event, string approverId)
        {
            if (!this.approversByEvent.ContainsKey(@event.EventId))
            {
                this.approversByEvent[@event.EventId] = new List<string>();
            }

            this.eventsById[@event.EventId] = @event;
            this.approversByEvent[@event.EventId].Add(approverId);
        }

        private void RemoveEvent(string eventId)
        {
            if (this.eventsById.ContainsKey(eventId))
            {
                this.eventsById.Remove(eventId);
            }

            if (this.approversByEvent.ContainsKey(eventId))
            {
                this.approversByEvent.Remove(eventId);
            }
        }

        private async Task<string> GetNextApproverId(CalendarEvent @event, IEnumerable<string> approvers)
        {
            var response = await this.calendarEventsApprovalsChecker
                .Ask<GetNextCalendarEventApprover.Response>(
                    new GetNextCalendarEventApprover(@event.EmployeeId, Enumerable.Empty<string>(), @event.Type, approvers));

            switch (response)
            {
                case GetNextCalendarEventApprover.SuccessResponse msg:
                    return msg.NextApproverEmployeeId;

                case GetNextCalendarEventApprover.ErrorResponse msg:
                    throw new Exception(msg.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private class RefreshPendingActions
        {
            public static readonly RefreshPendingActions Instance = new RefreshPendingActions();
        }

        public class RefreshEventPendingActionsSuccess
        {
            public RefreshEventPendingActionsSuccess(CalendarEvent @event, string nextApprover)
            {
                this.Event = @event;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public string NextApprover { get; }
        }

        public class RefreshEventPendingActionsFailure
        {
            public RefreshEventPendingActionsFailure(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }
    }
}
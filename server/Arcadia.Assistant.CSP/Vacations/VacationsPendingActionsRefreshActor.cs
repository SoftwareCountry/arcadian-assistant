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
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class VacationsPendingActionsRefreshActor : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly AppSettings settings;
        private readonly ActorSelection calendarEventsApprovalsChecker;
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly Dictionary<string, CalendarEvent> eventsById = new Dictionary<string, CalendarEvent>();
        private readonly Dictionary<string, List<string>> approversByEvent = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, ICancelable> refreshSchedulesByEvent = new Dictionary<string, ICancelable>();

        public VacationsPendingActionsRefreshActor(AppSettings settings)
        {
            this.settings = settings;
            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            Context.System.EventStream.Subscribe<CalendarEventAddedToPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemoved>(this.Self);
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<VacationsPendingActionsRefreshActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshPendingActions msg:
                    var @event = this.eventsById[msg.EventId];
                    var approvers = this.approversByEvent[@event.EventId];

                    this.GetNextApproverId(@event, approvers)
                            .PipeTo(
                                this.Self,
                                success: result => new RefreshPendingActionsSuccess(@event, result),
                                failure: error => new RefreshPendingActionsFailure(@event, error));

                    break;

                case RefreshPendingActionsSuccess msg:
                    if (msg.NextApprover != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }

                    break;

                case RefreshPendingActionsFailure msg:
                    this.logger.Warning($"An error occured while updating pending actions for event {msg.Event.EventId}. {msg.Exception}");
                    break;

                case CalendarEventAddedToPendingActions msg when msg.Event.Type == CalendarEventTypes.Vacation:
                    this.AddApprover(msg.Event, msg.ApproverId);
                    break;

                case CalendarEventAddedToPendingActions _:
                    break;

                case CalendarEventRemovedFromPendingActions msg when msg.Event.Type == CalendarEventTypes.Vacation:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                case CalendarEventRemovedFromPendingActions _:
                    break;

                case CalendarEventChanged msg when !msg.NewEvent.IsPending && msg.NewEvent.Type == CalendarEventTypes.Vacation:
                    this.RemoveEvent(msg.NewEvent.EventId);
                    break;

                case CalendarEventChanged msg when msg.NewEvent.Type == CalendarEventTypes.Vacation:
                    this.eventsById[msg.NewEvent.EventId] = msg.NewEvent;
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventApprovalsChanged msg when !msg.Event.IsPending && msg.Event.Type == CalendarEventTypes.Vacation:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                case CalendarEventApprovalsChanged _:
                    break;

                case CalendarEventRemoved msg when msg.Event.Type == CalendarEventTypes.Vacation:
                    this.RemoveEvent(msg.Event.EventId);
                    break;

                case CalendarEventRemoved _:
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

                var schedule = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                    //this.settings.VacationsPendingActionsRefresh,
                    //this.settings.VacationsPendingActionsRefresh,
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(1),
                    this.Self,
                    new RefreshPendingActions(@event.EventId),
                    this.Self);
                this.refreshSchedulesByEvent[@event.EventId] = schedule;
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

            if (this.refreshSchedulesByEvent.ContainsKey(eventId))
            {
                this.refreshSchedulesByEvent[eventId].Cancel();
                this.refreshSchedulesByEvent.Remove(eventId);
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
            public RefreshPendingActions(string eventId)
            {
                this.EventId = eventId;
            }

            public string EventId { get; }
        }

        private class RefreshPendingActionsSuccess
        {
            public RefreshPendingActionsSuccess(CalendarEvent @event, string nextApprover)
            {
                this.Event = @event;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public string NextApprover { get; }
        }

        private class RefreshPendingActionsFailure
        {
            public RefreshPendingActionsFailure(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }
    }
}
namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Akka.Actor;
    using Akka.Event;
    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Model;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly string employeeId;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private Dictionary<string, CalendarEvent> eventsById = new Dictionary<string, CalendarEvent>();
        private Dictionary<string, IEnumerable<Approval>> approvalsByEvent = new Dictionary<string, IEnumerable<Approval>>();

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            IRefreshInformation refreshInformation,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.employeeId = employeeId;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes),
                this.Self,
                Refresh.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Refresh _:
                    this.logger.Debug($"Updating vacations information for employee {this.employeeId}...");

                    this.vacationsSyncExecutor.GetVacations(this.employeeId)
                        .PipeTo(
                            this.Self,
                            success: result => new RefreshSuccess(result),
                            failure: err => new RefreshFailed(err)
                        );

                    break;

                case RefreshSuccess msg:
                    this.logger.Debug($"Vacations information for employee {this.employeeId} is updated");

                    this.eventsById.Clear();
                    this.approvalsByEvent.Clear();

                    foreach (var vacation in msg.Vacations)
                    {
                        this.eventsById[vacation.CalendarEvent.EventId] = vacation.CalendarEvent;
                        this.approvalsByEvent[vacation.CalendarEvent.EventId] = vacation.Approvals;
                    }

                    break;

                case RefreshFailed msg:
                    this.logger.Error(msg.Exception, $"Failed to load vacations information for employee {this.employeeId}: {msg.Exception.Message}");
                    break;

                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.employeeId, this.eventsById.Values.ToList()));
                    break;

                case GetCalendarEvent msg when this.eventsById.ContainsKey(msg.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.eventsById[msg.EventId]));
                    break;

                case GetCalendarEvent _:
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEventApprovals msg when this.approvalsByEvent.ContainsKey(msg.Event.EventId):
                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(this.approvalsByEvent[msg.Event.EventId]));
                    break;

                case GetCalendarEventApprovals msg:
                    this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Event with event id {msg.Event.EventId} is not found"));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private class Refresh
        {
            public static readonly Refresh Instance = new Refresh();
        }

        private class RefreshSuccess
        {
            public RefreshSuccess(IReadOnlyCollection<CalendarEventWithApprovals> vacations)
            {
                this.Vacations = vacations;
            }

            public IReadOnlyCollection<CalendarEventWithApprovals> Vacations { get; }
        }

        private class RefreshFailed
        {
            public RefreshFailed(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
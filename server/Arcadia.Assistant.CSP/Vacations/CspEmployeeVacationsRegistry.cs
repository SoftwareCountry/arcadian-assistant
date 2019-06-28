namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private const string CspVacationsRegistryActorPath = @"/user/organization/departments/departments-storage/csp-vacations-registry";

        private readonly string employeeId;
        private readonly IRefreshInformation refreshInformation;
        private readonly ActorSelection cspVacationsRegistryActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public CspEmployeeVacationsRegistry(
            string employeeId,
            IRefreshInformation refreshInformation)
        {
            this.employeeId = employeeId;
            this.refreshInformation = refreshInformation;

            this.cspVacationsRegistryActor = Context.ActorSelection(CspVacationsRegistryActorPath);

            this.Self.Tell(Initialize.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Initialize _:
                    this.GetVacations()
                        .PipeTo(
                            this.Self,
                            success: result => new Initialize.Success(result),
                            failure: error => new Initialize.Error(error));
                    break;

                case Initialize.Success msg:
                    foreach (var @event in msg.Events)
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event));
                    }

                    break;

                case Initialize.Error msg:
                    this.logger.Error(msg.Exception, "Error occured on vacations recover");

                    Context.System.Scheduler.ScheduleTellOnce(
                        TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                        this.Self,
                        Initialize.Instance,
                        this.Self);

                    break;

                case GetCalendarEvents _:
                    this.logger.Debug("GetCalendarEvents message received in CSP employee vacations registry");
                    this.cspVacationsRegistryActor.Tell(new CspVacationsRegistry.GetEmployeeCalendarEvents(this.employeeId), this.Sender);
                    break;

                case GetCalendarEvent msg:
                    this.cspVacationsRegistryActor.Tell(new CspVacationsRegistry.GetEmployeeCalendarEvent(this.employeeId, msg.EventId), this.Sender);
                    break;

                case GetCalendarEventApprovals msg:
                    this.cspVacationsRegistryActor.Tell(msg, this.Sender);
                    break;

                case InsertVacation msg:
                    this.cspVacationsRegistryActor.Tell(msg, this.Sender);
                    break;

                case UpdateVacation msg:
                    this.cspVacationsRegistryActor.Tell(msg, this.Sender);
                    break;

                case ApproveVacation msg:
                    this.cspVacationsRegistryActor.Tell(msg, this.Sender);
                    break;

                case CheckDatesAvailability msg:
                    this.CheckDatesAvailability(msg.Event)
                        .PipeTo(
                            this.Sender,
                            success: result => new CheckDatesAvailability.Success(result),
                            failure: err => new CheckDatesAvailability.Error(err));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<bool> CheckDatesAvailability(CalendarEvent @event)
        {
            var vacations = await this.GetVacations();

            var intersectedEventExists = vacations
                .Where(v => v.EventId != @event.EventId)
                .Any(v => v.Dates.DatesIntersectsWith(@event.Dates));
            return !intersectedEventExists;
        }

        private async Task<IEnumerable<CalendarEvent>> GetVacations()
        {
            var response = await this.cspVacationsRegistryActor.Ask<GetCalendarEvents.Response>(
                new CspVacationsRegistry.GetEmployeeCalendarEvents(this.employeeId));

            var vacations = response.Events
                .Where(@event => VacationStatuses.Actual.Contains(@event.Status)).ToList();

            return vacations;
        }

        private class Initialize
        {
            public static readonly Initialize Instance = new Initialize();

            public class Success
            {
                public Success(IEnumerable<CalendarEvent> events)
                {
                    this.Events = events;
                }

                public IEnumerable<CalendarEvent> Events { get; }
            }

            public class Error
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }
    }
}
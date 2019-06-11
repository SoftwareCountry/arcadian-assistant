namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeSickLeavesRegistry : UntypedActor, ILogReceive
    {
        private const string CspSickLeavesRegistryActorPath = @"/user/organization/departments/departments-storage/csp-sick-leaves-registry";

        private readonly string employeeId;
        private readonly IRefreshInformation refreshInformation;
        private readonly ActorSelection cspSickLeavesRegistryActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public CspEmployeeSickLeavesRegistry(
            string employeeId,
            IRefreshInformation refreshInformation)
        {
            this.employeeId = employeeId;
            this.refreshInformation = refreshInformation;

            this.cspSickLeavesRegistryActor = Context.ActorSelection(CspSickLeavesRegistryActorPath);

            this.Self.Tell(Initialize.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Initialize _:
                    this.GetSickLeaves()
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
                    this.logger.Error(msg.Exception, "Error occured on sick leaves recover");

                    Context.System.Scheduler.ScheduleTellOnce(
                        TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                        this.Self,
                        Initialize.Instance,
                        this.Self);

                    break;

                case GetCalendarEvents _:
                    this.cspSickLeavesRegistryActor.Tell(new CspSickLeavesRegistry.GetEmployeeCalendarEvents(this.employeeId), this.Sender);
                    break;

                case GetCalendarEvent msg:
                    this.cspSickLeavesRegistryActor.Tell(new CspSickLeavesRegistry.GetEmployeeCalendarEvent(this.employeeId, msg.EventId), this.Sender);
                    break;

                case InsertSickLeave msg:
                    this.cspSickLeavesRegistryActor.Tell(msg, this.Sender);
                    break;

                case UpdateSickLeave msg:
                    this.cspSickLeavesRegistryActor.Tell(msg, this.Sender);
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
            var sickLeaves = await this.GetSickLeaves();

            var intersectedEventExists = sickLeaves
                .Where(v => v.EventId != @event.EventId)
                .Any(v => v.Dates.DatesIntersectsWith(@event.Dates));
            return !intersectedEventExists;
        }

        private async Task<IEnumerable<CalendarEvent>> GetSickLeaves()
        {
            var response = await this.cspSickLeavesRegistryActor.Ask<GetCalendarEvents.Response>(
                new CspSickLeavesRegistry.GetEmployeeCalendarEvents(this.employeeId));

            var sickLeaves = response.Events
                .Where(@event => SickLeaveStatuses.Actual.Contains(@event.Status)).ToList();

            return sickLeaves;
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
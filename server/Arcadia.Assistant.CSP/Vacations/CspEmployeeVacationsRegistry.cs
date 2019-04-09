namespace Arcadia.Assistant.CSP.Vacations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private const string CspVacationsRegistryActorPath = @"/user/organization/departments/departments-storage/csp-vacations-registry";

        private readonly string employeeId;
        private readonly ActorSelection cspVacationsRegistryActor;

        public CspEmployeeVacationsRegistry(string employeeId)
        {
            this.employeeId = employeeId;

            this.cspVacationsRegistryActor = Context.ActorSelection(CspVacationsRegistryActorPath);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
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

        private async Task<IEnumerable<CalendarEvent>> GetVacations(bool onlyActual = true)
        {
            var response = await this.cspVacationsRegistryActor.Ask<GetCalendarEvents.Response>(
                new CspVacationsRegistry.GetEmployeeCalendarEvents(this.employeeId));
            var vacations = response.Events;

            if (onlyActual)
            {
                vacations = vacations.Where(@event => VacationStatuses.Actual.Contains(@event.Status)).ToList();
            }

            return vacations;
        }
    }
}
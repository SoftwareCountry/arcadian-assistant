namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class CspEmployeeSickLeavesRegistry : UntypedActor, ILogReceive
    {
        private const string CspSickLeavesRegistryActorPath = @"/user/organization/departments/departments-storage/csp-sick-leaves-registry";

        private readonly string employeeId;
        private readonly ActorSelection cspSickLeavesRegistryActor;

        public CspEmployeeSickLeavesRegistry(string employeeId)
        {
            this.employeeId = employeeId;

            this.cspSickLeavesRegistryActor = Context.ActorSelection(CspSickLeavesRegistryActorPath);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
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

        private async Task<IEnumerable<CalendarEvent>> GetSickLeaves(bool onlyActual = true)
        {
            var response = await this.cspSickLeavesRegistryActor.Ask<GetCalendarEvents.Response>(
                new CspSickLeavesRegistry.GetEmployeeCalendarEvents(this.employeeId));
            var sickLeaves = response.Events;

            if (onlyActual)
            {
                sickLeaves = sickLeaves.Where(@event => SickLeaveStatuses.Actual.Contains(@event.Status)).ToList();
            }

            return sickLeaves;
        }
    }
}
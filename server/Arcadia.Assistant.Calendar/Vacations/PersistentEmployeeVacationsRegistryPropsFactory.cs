namespace Arcadia.Assistant.Calendar.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions;

    public class PersistentEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        public Props CreateProps(string employeeId, IActorRef calendarEventsApprovalsChecker)
        {
            return Props.Create(() => new PersistentEmployeeVacationsRegistry(
                employeeId,
                calendarEventsApprovalsChecker));
        }
    }
}
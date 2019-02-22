namespace Arcadia.Assistant.Calendar.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;

    public class PersistentEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new PersistentEmployeeVacationsRegistry(employeeId));
        }
    }
}
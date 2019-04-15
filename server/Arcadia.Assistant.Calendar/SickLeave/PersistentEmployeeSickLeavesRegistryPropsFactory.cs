namespace Arcadia.Assistant.Calendar.SickLeave
{
    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;

    public class PersistentEmployeeSickLeavesRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new PersistentEmployeeSickLeavesRegistry(employeeId));
        }
    }
}
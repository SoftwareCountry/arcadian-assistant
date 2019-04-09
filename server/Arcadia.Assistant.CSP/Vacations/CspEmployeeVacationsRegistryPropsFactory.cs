namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(employeeId));
        }
    }
}
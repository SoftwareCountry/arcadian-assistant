namespace Arcadia.Assistant.CSP.SickLeaves
{
    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;

    public class CspEmployeeSickLeavesRegistryPropsFactory : IEmployeeSickLeavesRegistryPropsFactory
    {
        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeSickLeavesRegistry(employeeId));
        }
    }
}
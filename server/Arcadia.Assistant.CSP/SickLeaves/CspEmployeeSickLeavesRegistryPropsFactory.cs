namespace Arcadia.Assistant.CSP.SickLeaves
{
    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeSickLeavesRegistryPropsFactory : IEmployeeSickLeavesRegistryPropsFactory
    {
        private readonly IRefreshInformation refreshInformation;

        public CspEmployeeSickLeavesRegistryPropsFactory(IRefreshInformation refreshInformation)
        {
            this.refreshInformation = refreshInformation;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeSickLeavesRegistry(employeeId, this.refreshInformation));
        }
    }
}
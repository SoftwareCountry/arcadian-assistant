namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly IRefreshInformation refreshInformation;

        public CspEmployeeVacationsRegistryPropsFactory(IRefreshInformation refreshInformation)
        {
            this.refreshInformation = refreshInformation;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(employeeId, this.refreshInformation));
        }
    }
}
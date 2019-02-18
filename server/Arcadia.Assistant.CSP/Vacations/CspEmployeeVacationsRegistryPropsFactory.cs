namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly IRefreshInformation refreshInformation;

        public CspEmployeeVacationsRegistryPropsFactory(
            VacationsSyncExecutor vacationsSyncExecutor,
            IRefreshInformation refreshInformation)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.refreshInformation = refreshInformation;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(
                this.vacationsSyncExecutor,
                this.refreshInformation,
                employeeId));
        }
    }
}
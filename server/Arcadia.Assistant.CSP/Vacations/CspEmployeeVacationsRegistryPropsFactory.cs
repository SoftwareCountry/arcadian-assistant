namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly AccountingReminderConfiguration reminderConfiguration;
        private readonly IRefreshInformation refreshInformation;

        public CspEmployeeVacationsRegistryPropsFactory(
            VacationsSyncExecutor vacationsSyncExecutor,
            AccountingReminderConfiguration reminderConfiguration,
            IRefreshInformation refreshInformation)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.reminderConfiguration = reminderConfiguration;
            this.refreshInformation = refreshInformation;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(
                this.vacationsSyncExecutor,
                this.reminderConfiguration,
                this.refreshInformation,
                employeeId));
        }
    }
}
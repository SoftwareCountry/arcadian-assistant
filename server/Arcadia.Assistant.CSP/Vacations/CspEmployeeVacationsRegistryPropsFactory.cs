namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.CSP.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly AccountingReminderConfiguration reminderConfiguration;

        public CspEmployeeVacationsRegistryPropsFactory(
            VacationsSyncExecutor vacationsSyncExecutor,
            AccountingReminderConfiguration reminderConfiguration)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.reminderConfiguration = reminderConfiguration;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(
                this.vacationsSyncExecutor,
                this.reminderConfiguration,
                employeeId));
        }
    }
}
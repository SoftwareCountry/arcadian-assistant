namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.CSP.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly AccountingReminderConfiguration reminderConfiguration;

        public CspEmployeeVacationsRegistryPropsFactory(
            AccountingReminderConfiguration reminderConfiguration)
        {
            this.reminderConfiguration = reminderConfiguration;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(
                this.reminderConfiguration,
                employeeId));
        }
    }
}
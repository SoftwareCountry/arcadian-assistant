namespace Arcadia.Assistant.CSP.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistryPropsFactory : IEmployeeVacationsRegistryPropsFactory
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;

        public CspEmployeeVacationsRegistryPropsFactory(VacationsSyncExecutor vacationsSyncExecutor)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
        }

        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new CspEmployeeVacationsRegistry(
                this.vacationsSyncExecutor,
                employeeId));
        }
    }
}
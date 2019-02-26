namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Arcadia.Assistant.Configuration.Configuration;
    using Health.Abstractions;

    public class ArcadiaHealthChecker : HealthChecker
    {
        private const string VacationRegistryActorPath = @"/user/organization/employees/vacations-credit-registry";
        private const string DepartmentStorageActorPath = @"/user/organization/departments/departments-storage";
        private const string EmployeesInfoStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly string[] HealthStateNames = new[] { "Vacations Registry", "Departments Storage", "Employees Info Storage" };

        private readonly AppSettings settings;

        public ArcadiaHealthChecker(AppSettings settings)
        {
            this.settings = settings;
        }

        protected override async Task<HealthCheckMessageResponse> GetHealthStates()
        {
            var vacationsCreditRegistry = Context.ActorSelection(VacationRegistryActorPath);
            var departmentsStorage = Context.ActorSelection(DepartmentStorageActorPath);
            var employeesInfoStorage = Context.ActorSelection(EmployeesInfoStorageActorPath);

            var result = await Task.WhenAll(
                this.GetActorHealthState(vacationsCreditRegistry),
                this.GetActorHealthState(departmentsStorage),
                this.GetActorHealthState(employeesInfoStorage));

            var healthStates = result
                .Select((x, i) => new
                {
                    Name = HealthStateNames[i],
                    State = x
                })
                .ToDictionary(x => x.Name, x => x.State);

            return new HealthCheckMessageResponse(healthStates);
        }

        private async Task<HealthState> GetActorHealthState(ActorSelection actor)
        {
            try
            {
                var result = await actor.Ask<GetHealthCheckStatusMessage.GetHealthCheckStatusResponse>(
                    GetHealthCheckStatusMessage.Instance,
                    settings.Timeout);
                return new HealthState(result.Message == null, result.Message);
            }
            catch (Exception ex)
            {
                return new HealthState(false, ex.ToString());
            }
        }
    }
}
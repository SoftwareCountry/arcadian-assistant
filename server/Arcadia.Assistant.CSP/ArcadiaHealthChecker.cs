namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Health.Abstractions;
    using Organization.Abstractions;

    public class ArcadiaHealthChecker : HealthChecker
    {
        private const string HealthStateName = "Vacations Registry";
        private readonly IActorRef vacationsRegistry;

        public ArcadiaHealthChecker()
        {
            vacationsRegistry = Context.ActorOf(VacationsRegistry.GetProps, "vacations-registry");
        }

        protected override async Task<HealthCheckMessageResponse> GetHealthStates()
        {
            try
            {
                var result = await this.vacationsRegistry.Ask<GetVacationRegistryStatusMessage.GetVacationRegistryStatusResponse>(
                    GetVacationRegistryStatusMessage.Instance);
                return new HealthCheckMessageResponse(this.GetHealthResultDictionary(result.Message));
            }
            catch (Exception ex)
            {
                return new HealthCheckMessageResponse(this.GetHealthResultDictionary(ex.Message));
            }
        }

        private IDictionary<string, HealthState> GetHealthResultDictionary(string errorMessage)
        {
            return new Dictionary<string, HealthState>
            {
                { HealthStateName, new HealthState(errorMessage == null, errorMessage) }
            };
        }
    }
}
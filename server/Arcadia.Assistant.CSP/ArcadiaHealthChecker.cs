namespace Arcadia.Assistant.CSP
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Health.Abstractions;
    using Organization.Abstractions;

    public class ArcadiaHealthChecker : HealthChecker, ILogReceive
    {
        private const string HealthStateName = "Vacations Registry";
        private readonly IActorRef vacationsRegistry;

        public ArcadiaHealthChecker()
        {
            vacationsRegistry = Context.ActorOf(VacationsRegistry.GetProps, "vacations-registry");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HealthCheckMessage _:
                    this.vacationsRegistry.Ask<GetVacationRegistryStatusMessage.GetVacationRegistryStatusResponse>(
                            GetVacationRegistryStatusMessage.Instance)
                        .PipeTo(
                            this.Sender,
                            success: x => new HealthCheckMessageResponse(this.GetHealthResultDictionary(x.Message)),
                            failure: ex => new HealthCheckMessageResponse(this.GetHealthResultDictionary(ex.Message)));
                    break;

                default:
                    Unhandled(message);
                    break;
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
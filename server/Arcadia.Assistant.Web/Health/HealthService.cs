namespace Arcadia.Assistant.Web.Health
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Assistant.Health.Abstractions;
    using Configuration;
    using Server.Interop;

    public class HealthService : IHealthService
    {
        private readonly IActorRefFactory actorSystem;
        private readonly ActorPathsBuilder pathsBuilder;
        private readonly ITimeoutSettings timeoutSettings;

        public HealthService(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder, ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<IDictionary<string, bool>> GetHealthState(CancellationToken cancellationToken)
        {
            var healthActor = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Health));

            try
            {
                var healthCheckResponse = await healthActor.Ask<HealthCheckMessageResponse>(
                    HealthCheckMessage.Instance,
                    this.timeoutSettings.Timeout * 2,
                    cancellationToken);

                // What to do if concrete implementation already returned dictionary with such key?
                healthCheckResponse.HealthState[WellKnownHealthStateName.Server] = true;

                return healthCheckResponse.HealthState;
            }
            catch (Exception)
            {
                return new Dictionary<string, bool>
                {
                    { WellKnownHealthStateName.Server, false }
                };
            }
        }
    }
}
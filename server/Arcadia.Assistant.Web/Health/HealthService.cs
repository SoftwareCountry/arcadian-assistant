namespace Arcadia.Assistant.Web.Health
{
    using System;
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

        public Task<bool> GetIsServerAlive(CancellationToken cancellationToken)
        {
            var health = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Health));
            var isServerAliveTask = health.Ask<bool>(
                new HealthCheckMessage(HealthCheckType.Server),
                this.timeoutSettings.Timeout,
                cancellationToken);

            return this.WrapCheckAliveTask(isServerAliveTask);
        }

        public Task<bool> GetIs1CAlive(CancellationToken cancellationToken)
        {
            var health = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Health));
            var is1CAliveTask = health.Ask<bool>(
                new HealthCheckMessage(HealthCheckType.Check1C),
                this.timeoutSettings.Timeout,
                cancellationToken);

            return this.WrapCheckAliveTask(is1CAliveTask);
        }

        public Task<bool> GetIsDatabaseAlive(CancellationToken cancellationToken)
        {
            var health = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Health));
            var isDatabaseAliveTask = health.Ask<bool>(
                new HealthCheckMessage(HealthCheckType.CheckDatabase),
                this.timeoutSettings.Timeout,
                cancellationToken);

            return this.WrapCheckAliveTask(isDatabaseAliveTask);
        }

        private async Task<bool> WrapCheckAliveTask(Task<bool> checkAliveTask)
        {
            try
            {
                return await checkAliveTask;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
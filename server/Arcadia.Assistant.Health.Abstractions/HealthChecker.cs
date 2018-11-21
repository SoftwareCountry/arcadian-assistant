namespace Arcadia.Assistant.Health.Abstractions
{
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class HealthChecker : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HealthCheckMessage _:
                    this.GetHealthStates().PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<HealthCheckMessageResponse> GetHealthStates();
    }
}
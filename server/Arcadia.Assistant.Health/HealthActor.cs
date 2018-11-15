namespace Arcadia.Assistant.Health
{
    using Abstractions;
    using Akka.Actor;

    public class HealthActor : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            if (!(message is HealthCheckMessage healthMessage))
            {
                this.Unhandled(message);
                return;
            }

            switch (healthMessage.CheckType)
            {
                case HealthCheckType.Server:
                    this.Sender.Tell(true);
                    break;

                case HealthCheckType.Check1C:
                    this.Sender.Tell(true);
                    break;

                case HealthCheckType.CheckDatabase:
                    this.Sender.Tell(true);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}
namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ValueActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            this.Sender.Tell(new[] { "test", "test2" });
        }
    }
}
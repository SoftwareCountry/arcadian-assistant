namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;

    public class DispatcherActor : UntypedActor, ILogReceive
    {
        private readonly ServerActorsCollection serverActors;

        public DispatcherActor(ServerActorsCollection serverActors)
        {
            this.serverActors = serverActors;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Messages.Connect _:
                    this.Sender.Tell(new Messages.Connect.Response(this.serverActors));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}
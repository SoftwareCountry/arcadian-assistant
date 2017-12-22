namespace Arcadia.Assistant.Server.Interop
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(IActorRef helpdesk, IActorRef organization)
        {
            this.Helpdesk = helpdesk;
            this.Organization = organization;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Organization { get; }
    }
}
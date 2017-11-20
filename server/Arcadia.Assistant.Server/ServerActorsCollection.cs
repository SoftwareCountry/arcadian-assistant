namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(IActorRef helpdesk, IActorRef employees)
        {
            this.Helpdesk = helpdesk;
            this.Employees = employees;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Employees { get; }
    }
}
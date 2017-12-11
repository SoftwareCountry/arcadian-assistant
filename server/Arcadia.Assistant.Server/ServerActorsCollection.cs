namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(IActorRef helpdesk, IActorRef departments)
        {
            this.Helpdesk = helpdesk;
            this.Departments = departments;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Departments { get; }
    }
}
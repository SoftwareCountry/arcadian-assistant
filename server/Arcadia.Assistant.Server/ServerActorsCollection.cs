namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(IActorRef helpdesk, IActorRef health, IActorRef departments, IActorRef sharedFeedsActor)
        {
            this.Helpdesk = helpdesk;
            this.Health = health;
            this.Departments = departments;
            this.SharedFeedsActor = sharedFeedsActor;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Health { get; }

        public IActorRef Departments { get; }

        public IActorRef SharedFeedsActor { get; }
    }
}
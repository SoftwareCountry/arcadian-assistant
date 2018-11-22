namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(IActorRef helpdesk, IActorRef departments, IActorRef sharedFeedsActor, IActorRef userPreferences)
        {
            this.Helpdesk = helpdesk;
            this.Departments = departments;
            this.SharedFeedsActor = sharedFeedsActor;
            UserPreferences = userPreferences;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Departments { get; }

        public IActorRef SharedFeedsActor { get; }

        public IActorRef UserPreferences { get; }
    }
}
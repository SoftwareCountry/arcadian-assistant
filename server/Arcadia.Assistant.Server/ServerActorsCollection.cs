namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    public class ServerActorsCollection
    {
        public ServerActorsCollection(
            IActorRef helpdesk,
            IActorRef health,
            IActorRef departments,
            IActorRef sharedFeedsActor,
            IActorRef userPreferences,
            IActorRef pushNotificationsDevices)
        {
            this.Helpdesk = helpdesk;
            this.Health = health;
            this.Departments = departments;
            this.SharedFeedsActor = sharedFeedsActor;
            this.UserPreferences = userPreferences;
            this.PushNotificationsDevices = pushNotificationsDevices;
        }

        public IActorRef Helpdesk { get; }

        public IActorRef Health { get; }

        public IActorRef Departments { get; }

        public IActorRef SharedFeedsActor { get; }

        public IActorRef UserPreferences { get; }

        public IActorRef PushNotificationsDevices { get; }
    }
}
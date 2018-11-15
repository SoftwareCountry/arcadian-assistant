namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Health;

    public class ActorSystemBuilder
    {
        private readonly ActorSystem actorSystem;

        public ActorSystemBuilder(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public ServerActorsCollection AddRootActors()
        {
            var departments = this.actorSystem.ActorOf(this.actorSystem.DI().Props<OrganizationActor>(), WellKnownActorPaths.Organization);
            var health = this.actorSystem.ActorOf(this.actorSystem.DI().Props<HealthActor>(), WellKnownActorPaths.Health);
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            var feeds = this.actorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(departments)), WellKnownActorPaths.SharedFeeds);

            return new ServerActorsCollection(departments, health, helpdesk, feeds);
        }
    }
}
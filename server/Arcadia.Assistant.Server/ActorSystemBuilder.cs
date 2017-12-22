namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI;
    using Akka.DI.Core;

    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Server.Interop;

    public class ActorSystemBuilder
    {
        private readonly ActorSystem actorSystem;

        public ActorSystemBuilder(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public ServerActorsCollection AddRootActors()
        {
            var organization = this.actorSystem.ActorOf(this.actorSystem.DI().Props<OrganizationActor>(), "organization");
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), "helpdesk");

            var serverActors = new ServerActorsCollection(helpdesk, organization);

            var dispatcher = this.actorSystem.ActorOf(Props.Create(() => new DispatcherActor(serverActors)), DispatcherPath.DispatcherAgentName);

            return serverActors;
        }
    }
}
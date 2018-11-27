namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Calendar.SickLeave;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Health.Abstractions;
    using Arcadia.Assistant.UserPreferences;
    using Organization.Abstractions;

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
            var health = this.actorSystem.ActorOf(this.actorSystem.DI().Props<HealthChecker>(), WellKnownActorPaths.Health);
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            var feeds = this.actorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(departments)), WellKnownActorPaths.SharedFeeds);
            var userPreferences = this.actorSystem.ActorOf(this.actorSystem.DI().Props<UserPreferencesActor>(), WellKnownActorPaths.UserPreferences);

            var sickLeaveEmailActorProps = this.actorSystem.DI().Props<SendEmailSickLeaveActor>();
            this.actorSystem.ActorOf(Props.Create(() => new NotificationsActor(sickLeaveEmailActorProps)), "notifications");

            this.actorSystem.ActorOf(this.actorSystem.DI().Props<VacationApprovalsChecker>(), WellKnownActorPaths.VacationApprovalsChecker);

            return new ServerActorsCollection(departments, health, helpdesk, feeds, userPreferences);
        }
    }
}
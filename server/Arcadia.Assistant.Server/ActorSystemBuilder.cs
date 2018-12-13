namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Calendar.SickLeave;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Health.Abstractions;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.UserPreferences;

    public class ActorSystemBuilder
    {
        private readonly ActorSystem actorSystem;

        public ActorSystemBuilder(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public ServerActorsCollection AddRootActors(IEmailSettings emailSettings)
        {
            var organization = this.actorSystem.ActorOf(this.actorSystem.DI().Props<OrganizationActor>(), WellKnownActorPaths.Organization);
            var health = this.actorSystem.ActorOf(this.actorSystem.DI().Props<HealthChecker>(), WellKnownActorPaths.Health);
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            var feeds = this.actorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(organization)), WellKnownActorPaths.SharedFeeds);
            var userPreferences = this.actorSystem.ActorOf(this.actorSystem.DI().Props<UserPreferencesActor>(), WellKnownActorPaths.UserPreferences);
            var pushNotificationsDevices = this.actorSystem.ActorOf(Props.Create(() => new PushNotificationsDevicesActor()), WellKnownActorPaths.PushNotificationsDevices);

            this.actorSystem.ActorOf(Props.Create(() => new SendEmailSickLeaveActor(emailSettings, organization)), "sick-leave-email");

            var emailNotificationsActorProps = this.actorSystem.DI().Props<EmailNotificationsActor>();
            var pushNotificationsActorProps = this.actorSystem.DI().Props<PushNotificationsActor>();
            this.actorSystem.ActorOf(
                Props.Create(() => new NotificationsDispatcherActor(emailNotificationsActorProps, pushNotificationsActorProps)),
                "notifications");

            return new ServerActorsCollection(organization, health, helpdesk, feeds, userPreferences, pushNotificationsDevices);
        }
    }
}
namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Calendar.Notifications;
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

        public ServerActorsCollection AddRootActors(ICalendarEventsMessagingSettings calendarEventsMessagingSettings)
        {
            var organization = this.actorSystem.ActorOf(this.actorSystem.DI().Props<OrganizationActor>(), WellKnownActorPaths.Organization);
            var health = this.actorSystem.ActorOf(this.actorSystem.DI().Props<HealthChecker>(), WellKnownActorPaths.Health);
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            var feeds = this.actorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(organization)), WellKnownActorPaths.SharedFeeds);
            var userPreferences = this.actorSystem.ActorOf(this.actorSystem.DI().Props<UserPreferencesActor>(), WellKnownActorPaths.UserPreferences);
            var pushNotificationsDevices = this.actorSystem.ActorOf(Props.Create(() => new PushNotificationsDevicesActor()), WellKnownActorPaths.PushNotificationsDevices);

            this.actorSystem.ActorOf(
                Props.Create(() => new SickLeaveApprovedEmailNotificationActor(
                    calendarEventsMessagingSettings.SickLeaveApprovedEmail,
                    organization)),
                "sick-leave-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventAssignedToApproverEmailNotificationActor(
                    calendarEventsMessagingSettings.EventAssignedToApproverEmail,
                    organization,
                    userPreferences)),
                "event-assigned-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventAssignedToApproverPushNotificationActor(
                    userPreferences,
                    pushNotificationsDevices)),
                "event-assigned-push");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventStatusChangedEmailNotificationActor(
                    calendarEventsMessagingSettings.EventStatusChangedEmail,
                    organization,
                    userPreferences)),
                "event-changed-status-owner-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventUserGrantedApprovalEmailNotificationActor(
                    calendarEventsMessagingSettings.EventUserGrantedApprovalEmail,
                    organization,
                    userPreferences)),
                "event-granted-approval-owner-email");

            var emailNotificationsActorProps = this.actorSystem.DI().Props<EmailNotificationsActor>();
            var pushNotificationsActorProps = this.actorSystem.DI().Props<PushNotificationsActor>();
            this.actorSystem.ActorOf(
                Props.Create(() => new NotificationsDispatcherActor(emailNotificationsActorProps, pushNotificationsActorProps)),
                "notifications");

            return new ServerActorsCollection(organization, health, helpdesk, feeds, userPreferences, pushNotificationsDevices);
        }
    }
}
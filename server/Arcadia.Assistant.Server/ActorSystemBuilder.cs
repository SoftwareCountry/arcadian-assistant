namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Calendar.Notifications;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Health.Abstractions;
    using Arcadia.Assistant.InboxEmail;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Patterns;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.UserPreferences;

    public class ActorSystemBuilder
    {
        private readonly ActorSystem actorSystem;

        public ActorSystemBuilder(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public ServerActorsCollection AddRootActors(
            ICalendarEventsMailSettings calendarEventsMailSettings,
            ICalendarEventsPushSettings calendarEventsPushSettings,
            IImapSettings imapSettings)
        {
            var organization = this.actorSystem.ActorOf(this.actorSystem.DI().Props<OrganizationActor>(), WellKnownActorPaths.Organization);
            var health = this.actorSystem.ActorOf(this.actorSystem.DI().Props<HealthChecker>(), WellKnownActorPaths.Health);
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), WellKnownActorPaths.Helpdesk);
            var feeds = this.actorSystem.ActorOf(Props.Create(() => new SharedFeedsActor(organization)), WellKnownActorPaths.SharedFeeds);

            var userPreferenceActorProps = this.actorSystem.DI().Props<UserPreferencesActor>();
            var userPreferences = this.actorSystem.ActorOf(new PersistenceSupervisorFactory().Get(userPreferenceActorProps), WellKnownActorPaths.UserPreferences);
            var pushNotificationsDevices = this.actorSystem.ActorOf(PushNotificationsDevicesActor.CreateProps(), WellKnownActorPaths.PushNotificationsDevices);

            var inboxEmailsActor = this.actorSystem.ActorOf(this.actorSystem.DI().Props<InboxEmailActor>(), "inbox-emails");
            this.actorSystem.ActorOf(Props.Create(() => new InboxEmailsNotificator(imapSettings, inboxEmailsActor)), "emails-notificator");

            this.CreateCalendarEventNotificationActors(
                calendarEventsMailSettings,
                calendarEventsPushSettings,
                organization,
                userPreferences,
                pushNotificationsDevices);

            var emailNotificationsActorProps = this.actorSystem.DI().Props<EmailNotificationsActor>();
            var pushNotificationsActorProps = this.actorSystem.DI().Props<PushNotificationsActor>();
            this.actorSystem.ActorOf(
                Props.Create(() => new NotificationsDispatcherActor(emailNotificationsActorProps, pushNotificationsActorProps)),
                "notifications");

            return new ServerActorsCollection(organization, health, helpdesk, feeds, userPreferences, pushNotificationsDevices);
        }

        private void CreateCalendarEventNotificationActors(
            ICalendarEventsMailSettings calendarEventsMailSettings,
            ICalendarEventsPushSettings calendarEventsPushSettings,
            IActorRef organization,
            IActorRef userPreferences,
            IActorRef pushNotificationsDevices)
        {
            this.actorSystem.ActorOf(
                Props.Create(() => new SickLeaveApprovedEmailNotificationActor(
                    calendarEventsMailSettings.SickLeaveApproved,
                    organization)),
                "sick-leave-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventAssignedToApproverEmailNotificationActor(
                    calendarEventsMailSettings.EventAssignedToApprover,
                    organization,
                    userPreferences)),
                "event-assigned-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventAssignedToApproverPushNotificationActor(
                    calendarEventsPushSettings.EventAssignedToApprover,
                    organization,
                    userPreferences,
                    pushNotificationsDevices)),
                "event-assigned-push");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventStatusChangedEmailNotificationActor(
                    calendarEventsMailSettings.EventStatusChanged,
                    organization,
                    userPreferences)),
                "event-changed-status-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventStatusChangedPushNotificationActor(
                    calendarEventsPushSettings.EventStatusChanged,
                    userPreferences,
                    pushNotificationsDevices)),
                "event-changed-status-push");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventUserGrantedApprovalEmailNotificationActor(
                    calendarEventsMailSettings.EventUserGrantedApproval,
                    organization,
                    userPreferences)),
                "event-granted-approval-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventUserGrantedApprovalPushNotificationActor(
                    calendarEventsPushSettings.EventUserGrantedApproval,
                    organization,
                    userPreferences,
                    pushNotificationsDevices)),
                "event-granted-approval-push");
        }
    }
}
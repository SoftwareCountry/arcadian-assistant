namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.ApplicationBuilds;
    using Arcadia.Assistant.Calendar;
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
    using Arcadia.Assistant.Organization.Abstractions;
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

            this.actorSystem.ActorOf(Props.Create(() => new ApplicationBuildsActor()), WellKnownActorPaths.ApplicationBuilds);

            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var userPreferenceActorProps = this.actorSystem.DI().Props<UserPreferencesActor>();
            var userPreferences = this.actorSystem.ActorOf(
                persistenceSupervisorFactory.Get(userPreferenceActorProps),
                WellKnownActorPaths.UserPreferences);

            var pushNotificationsDevicesActorProps = PushNotificationsDevicesActor.CreateProps();
            var pushNotificationsDevices = this.actorSystem.ActorOf(
                persistenceSupervisorFactory.Get(pushNotificationsDevicesActorProps),
                WellKnownActorPaths.PushNotificationsDevices);

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

            this.actorSystem.ActorOf(this.actorSystem.DI().Props<CalendarEventsApprovalsChecker>(), "calendar-events-approvals");

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
                Props.Create(() => new SickLeaveAccountingEmailNotificationActor(
                    calendarEventsMailSettings.SickLeaveCreated,
                    calendarEventsMailSettings.SickLeaveProlonged,
                    calendarEventsMailSettings.SickLeaveCancelled,
                    organization)),
                "sick-leave-accounting-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new SickLeaveManagerEmailNotificationActor(
                    calendarEventsMailSettings.SickLeaveCreated,
                    calendarEventsMailSettings.SickLeaveProlonged,
                    calendarEventsMailSettings.SickLeaveCancelled,
                    organization,
                    userPreferences)),
                    "sick-leave-manager-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new SickLeaveManagerPushNotificationActor(
                    calendarEventsPushSettings.SickLeaveCreatedManager,
                    calendarEventsPushSettings.SickLeaveProlongedManager,
                    calendarEventsPushSettings.SickLeaveCancelledManager,
                    organization,
                    userPreferences,
                    pushNotificationsDevices)),
                "sick-leave-manager-push");
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
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

            this.actorSystem.ActorOf(
                Props.Create(() => new SickLeaveApprovedNotificationActor(
                    calendarEventsMessagingSettings.SickLeaveApproved, 
                    organization)),
                "sick-leave-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventAssignedToApproverNotificationActor(
                    calendarEventsMessagingSettings.EventAssignedToApprover, 
                    organization,
                    userPreferences)),
                "event-assigned-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new OwnerEventStatusChangedNotificationActor(
                    calendarEventsMessagingSettings.EventChangedByOwner,
                    organization,
                    userPreferences)),
                "event-changed-status-owner-email");
            this.actorSystem.ActorOf(
                Props.Create(() => new EventUserGrantedApprovalNotificationActor(
                    calendarEventsMessagingSettings.EventUserGrantedApproval,
                    organization,
                    userPreferences)),
                "event-granted-approval-owner-email");

            var emailNotificationsActorProps = this.actorSystem.DI().Props<EmailNotificationsActor>();
            this.actorSystem.ActorOf(Props.Create(() => new NotificationsDispatcherActor(emailNotificationsActorProps)), "notifications");

            return new ServerActorsCollection(organization, health, helpdesk, feeds, userPreferences);
        }
    }
}
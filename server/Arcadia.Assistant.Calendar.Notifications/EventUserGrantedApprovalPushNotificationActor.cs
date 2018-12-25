namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    using PushNotification = Arcadia.Assistant.Notifications.Push.PushNotification;

    public class EventUserGrantedApprovalPushNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IPushNotification pushNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;
        private readonly IActorRef pushDevicesActor;
        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventUserGrantedApprovalPushNotificationActor(
            IPushNotification pushNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor,
            IActorRef pushDevicesActor)
        {
            this.pushNotificationConfig = pushNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;
            this.pushDevicesActor = pushDevicesActor;

            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventApprovalsChanged msg when msg.Approvals.Count() != 0:
                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (ownerPreferencesResult, ownerPushTokensResult, approverEmployeeResult) = task.Result;

                            return new CalendarEventApprovalsChangedWithAdditionalData(
                                msg.Event,
                                ownerPreferencesResult.UserPreferences,
                                ownerPushTokensResult.DevicePushTokens,
                                approverEmployeeResult.Employees.First().Metadata);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventApprovalsChanged _:
                    break;

                case CalendarEventApprovalsChangedWithAdditionalData msg
                    when msg.OwnerUserPreferences.EmailNotifications:

                    this.logger.Debug("Sending email notification about user {0} granted approval for event {1} of {2}",
                        msg.Approver.EmployeeId, msg.Event.EventId, msg.Event.EmployeeId);

                    var pushNotification = this.CreatePushNotification(msg);
                    Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));

                    break;

                case CalendarEventApprovalsChangedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(GetUserPreferencesMessage.Response, GetDevicePushTokens.Success, EmployeesQuery.Response)>
            GetAdditionalData(CalendarEventApprovalsChanged message)
        {
            var lastApproval = message.Approvals
                .OrderByDescending(a => a.Timestamp)
                .First();

            var ownerPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.Event.EmployeeId));
            var ownerPushTokensTask = this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(message.Event.EmployeeId));
            var approverEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(lastApproval.ApprovedBy));

            await Task.WhenAll(ownerPreferencesTask, ownerPushTokensTask, approverEmployeeTask);
            return (ownerPreferencesTask.Result, ownerPushTokensTask.Result, approverEmployeeTask.Result);
        }

        private PushNotification CreatePushNotification(CalendarEventApprovalsChangedWithAdditionalData message)
        {
            var content = new PushNotificationContent
            {
                Title = this.pushNotificationConfig.Title,
                Body = string.Format(this.pushNotificationConfig.Body, message.Event.Type, message.Approver.Name),
                CustomData = new
                {
                    message.Event.EventId,
                    message.Event.EmployeeId,
                    Type = CalendarEventPushNotificationTypes.EventUserGrantedApproval
                }
            };

            return new PushNotification(content, message.OwnerPushTokens.ToList());
        }

        private class CalendarEventApprovalsChangedWithAdditionalData
        {
            public CalendarEventApprovalsChangedWithAdditionalData(
                CalendarEvent @event,
                UserPreferences ownerUserPreferences,
                IEnumerable<DevicePushToken> ownerPushTokens,
                EmployeeMetadata approver)
            {
                this.Event = @event;
                this.OwnerUserPreferences = ownerUserPreferences;
                this.OwnerPushTokens = ownerPushTokens;
                this.Approver = approver;
            }

            public CalendarEvent Event { get; }

            public UserPreferences OwnerUserPreferences { get; }

            public IEnumerable<DevicePushToken> OwnerPushTokens { get; }

            public EmployeeMetadata Approver { get; }
        }
    }
}
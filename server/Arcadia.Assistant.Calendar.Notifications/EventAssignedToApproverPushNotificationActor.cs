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

    public class EventAssignedToApproverPushNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IPushNotification pushNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;
        private readonly IActorRef pushDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventAssignedToApproverPushNotificationActor(
            IPushNotification pushNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor,
            IActorRef pushDevicesActor)
        {
            this.pushNotificationConfig = pushNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;
            this.pushDevicesActor = pushDevicesActor;

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventAssignedToApprover msg:
                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (ownerResult, approverPreferencesResult, approverPushTokensResult) = task.Result;

                            return new CalendarEventAssignedWithAdditionalData(
                                msg.Event,
                                msg.ApproverId,
                                ownerResult.Employees.First().Metadata,
                                approverPreferencesResult.UserPreferences,
                                approverPushTokensResult.DevicePushTokens);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventAssignedWithAdditionalData msg
                    when msg.ApproverUserPreferences.PushNotifications:

                    this.logger.Debug("Sending push notification about event {0} of {1} assigned to {2}",
                        msg.Event.EventId, msg.Event.EmployeeId, msg.ApproverId);

                    var pushNotification = this.CreatePushNotification(msg);
                    Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));

                    break;

                case CalendarEventAssignedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(EmployeesQuery.Response, GetUserPreferencesMessage.Response, GetDevicePushTokens.Success)>
            GetAdditionalData(CalendarEventAssignedToApprover message)
        {
            var ownerEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(message.Event.EmployeeId));
            var approverPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.ApproverId));
            var approverPushTokensTask = this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(message.ApproverId));

            await Task.WhenAll(ownerEmployeeTask, approverPreferencesTask, approverPushTokensTask);
            return (ownerEmployeeTask.Result, approverPreferencesTask.Result, approverPushTokensTask.Result);
        }

        private PushNotification CreatePushNotification(CalendarEventAssignedWithAdditionalData message)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["eventType"] = message.Event.Type,
                ["employee"] = message.Owner.Name
            };

            templateExpressionContext = new DictionaryMerge().Perform(templateExpressionContext, message.Event.AdditionalData);

            var content = new PushNotificationContent
            {
                Title = this.pushNotificationConfig.Title,
                Body = new TemplateExpressionParser().Parse(this.pushNotificationConfig.Body, templateExpressionContext),
                CustomData = new
                {
                    message.Event.EventId,
                    message.Event.EmployeeId,
                    message.ApproverId,
                    Type = CalendarEventPushNotificationTypes.EventAssignedToApprover
                }
            };

            return new PushNotification(content, message.ApproverPushTokens.ToList());
        }

        private class CalendarEventAssignedWithAdditionalData
        {
            public CalendarEventAssignedWithAdditionalData(
                CalendarEvent @event,
                string approverId,
                EmployeeMetadata owner,
                UserPreferences approverUserPreferences,
                IEnumerable<DevicePushToken> approverPushTokens)
            {
                this.Event = @event;
                this.ApproverId = approverId;
                this.Owner = owner;
                this.ApproverUserPreferences = approverUserPreferences;
                this.ApproverPushTokens = approverPushTokens;
            }

            public CalendarEvent Event { get; }

            public string ApproverId { get; }

            public EmployeeMetadata Owner { get; }

            public UserPreferences ApproverUserPreferences { get; }

            public IEnumerable<DevicePushToken> ApproverPushTokens { get; }
        }
    }
}
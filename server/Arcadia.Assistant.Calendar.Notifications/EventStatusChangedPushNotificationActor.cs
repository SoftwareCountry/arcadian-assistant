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
    using Arcadia.Assistant.UserPreferences;

    using PushNotification = Arcadia.Assistant.Notifications.Push.PushNotification;

    public class EventStatusChangedPushNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IPushNotification pushNotificationConfig;
        private readonly IActorRef userPreferencesActor;
        private readonly IActorRef pushDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventStatusChangedPushNotificationActor(
            IPushNotification pushNotificationConfig,
            IActorRef userPreferencesActor,
            IActorRef pushDevicesActor)
        {
            this.pushNotificationConfig = pushNotificationConfig;
            this.userPreferencesActor = userPreferencesActor;
            this.pushDevicesActor = pushDevicesActor;

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg
                    when msg.NewEvent.Status != msg.OldEvent.Status && msg.UpdatedBy != msg.NewEvent.EmployeeId:

                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (preferencesResult, pushTokensResult) = task.Result;

                            return new CalendarEventChangedWithAdditionalData(
                                msg.NewEvent,
                                preferencesResult.UserPreferences,
                                pushTokensResult.DevicePushTokens);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventChangedWithAdditionalData msg
                    when msg.OwnerUserPreferences.PushNotifications:

                    this.logger.Debug("Sending push notification about event {0} status changed to owner", msg.Event.EventId);

                    var pushNotification = this.CreatePushNotification(msg);
                    Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));

                    break;

                case CalendarEventChangedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(GetUserPreferencesMessage.Response, GetDevicePushTokens.Success)>
            GetAdditionalData(CalendarEventChanged message)
        {
            var ownerPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.NewEvent.EmployeeId));
            var ownerPushTokensTask = this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(message.NewEvent.EmployeeId));

            await Task.WhenAll(ownerPreferencesTask, ownerPushTokensTask);
            return (ownerPreferencesTask.Result, ownerPushTokensTask.Result);
        }

        private PushNotification CreatePushNotification(CalendarEventChangedWithAdditionalData message)
        {
            var content = new PushNotificationContent
            {
                Title = this.pushNotificationConfig.Title,
                Body = string.Format(this.pushNotificationConfig.Body, message.Event.Type, message.Event.Status),
                CustomData = new
                {
                    message.Event.EventId,
                    message.Event.EmployeeId,
                    Type = CalendarEventPushNotificationTypes.EventStatusChanged
                }
            };

            return new PushNotification(content, message.OwnerPushTokens.ToList());
        }

        private class CalendarEventChangedWithAdditionalData
        {
            public CalendarEventChangedWithAdditionalData(CalendarEvent @event,
                UserPreferences ownerUserPreferences,
                IEnumerable<DevicePushToken> ownerPushTokens)
            {
                this.Event = @event;
                this.OwnerUserPreferences = ownerUserPreferences;
                this.OwnerPushTokens = ownerPushTokens;
            }

            public CalendarEvent Event { get; }

            public UserPreferences OwnerUserPreferences { get; }

            public IEnumerable<DevicePushToken> OwnerPushTokens { get; }
        }
    }
}
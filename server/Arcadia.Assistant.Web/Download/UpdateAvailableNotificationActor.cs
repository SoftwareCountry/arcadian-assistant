namespace Arcadia.Assistant.Web.Download
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    using NLog;

    public class UpdateAvailableNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IUpdateNotificationSettings updateNotificationSettings;
        private const string UpdateAvailablePushNotificationType = "UpdateAvailable";

        private readonly ActorSelection pushNotificationsDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();
        private readonly Logger nlogLogger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ApplicationTypeEnum, string> pushDeviceTypeByApplicationType =
            new Dictionary<ApplicationTypeEnum, string>
            {
                [ApplicationTypeEnum.Android] = PushDeviceTypes.Android,
                [ApplicationTypeEnum.Ios] = PushDeviceTypes.Ios
            };

        public UpdateAvailableNotificationActor(
            ActorPathsBuilder actorPathsBuilder,
            IUpdateNotificationSettings updateNotificationSettings)
        {
            this.updateNotificationSettings = updateNotificationSettings;
            this.pushNotificationsDevicesActor = Context.ActorSelection(actorPathsBuilder.Get(WellKnownActorPaths.PushNotificationsDevices));

            Context.System.EventStream.Subscribe<UpdateAvailable>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case UpdateAvailable msg:
                    this.nlogLogger.Debug($"UpdateAvailable event received in notification actor for application {msg.ApplicationType}");

                    this.GetDevicesPushTokensByApplication(msg.ApplicationType)
                        .PipeTo(
                            this.Self,
                            success: result => new UpdateAvailableWithAdditionalData(msg.ApplicationType, result),
                            failure: err => new UpdateAvailableError(err));
                    break;

                case UpdateAvailableWithAdditionalData msg:
                    this.SendUpdateNotification(msg);
                    break;

                case UpdateAvailableError msg:
                    this.nlogLogger.Warn(msg.Exception.ToString());
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<IEnumerable<DevicePushToken>> GetDevicesPushTokensByApplication(ApplicationTypeEnum applicationType)
        {
            var response = await this.pushNotificationsDevicesActor.Ask<GetDevicePushTokensByApplication.Response>(
                new GetDevicePushTokensByApplication(this.pushDeviceTypeByApplicationType[applicationType]));
            return response.DevicePushTokens.ToArray();
        }

        private void SendUpdateNotification(UpdateAvailableWithAdditionalData message)
        {
            this.nlogLogger.Debug($"Sending push notification about update available for {message.ApplicationType} application");
            this.nlogLogger.Debug($"Recepients: {string.Join(", ", message.DevicesPushTokens)}");

            var pushNotification = this.CreatePushNotification(message.DevicesPushTokens);
            Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));
        }

        private PushNotification CreatePushNotification(IEnumerable<DevicePushToken> pushTokens)
        {
            var content = new PushNotificationContent
            {
                Title = this.updateNotificationSettings.NotificationTitle,
                Body = this.updateNotificationSettings.NotificationBody,
                CustomData = new
                {
                    Type = UpdateAvailablePushNotificationType
                }
            };

            return new PushNotification(content, pushTokens);
        }

        private class UpdateAvailableWithAdditionalData
        {
            public UpdateAvailableWithAdditionalData(ApplicationTypeEnum applicationType, IEnumerable<DevicePushToken> devicesPushTokens)
            {
                this.ApplicationType = applicationType;
                this.DevicesPushTokens = devicesPushTokens;
            }

            public ApplicationTypeEnum ApplicationType { get; }

            public IEnumerable<DevicePushToken> DevicesPushTokens { get; }
        }

        private class UpdateAvailableError
        {
            public UpdateAvailableError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
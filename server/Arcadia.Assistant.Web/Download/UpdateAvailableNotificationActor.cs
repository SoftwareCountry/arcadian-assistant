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

    public class UpdateAvailableNotificationActor : UntypedActor, ILogReceive
    {
        private const string UpdateAvailablePushNotificationType = "UpdateAvailable";

        private readonly ActorSelection pushNotificationsDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly Dictionary<ApplicationTypeEnum, string> pushDeviceTypeByApplicationType =
            new Dictionary<ApplicationTypeEnum, string>
            {
                [ApplicationTypeEnum.Android] = PushDeviceTypes.Android,
                [ApplicationTypeEnum.Ios] = PushDeviceTypes.Ios
            };

        public UpdateAvailableNotificationActor(ActorPathsBuilder actorPathsBuilder)
        {
            this.pushNotificationsDevicesActor = Context.ActorSelection(actorPathsBuilder.Get(WellKnownActorPaths.PushNotificationsDevices));

            Context.System.EventStream.Subscribe<UpdateAvailable>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case UpdateAvailable msg:
                    this.GetDevicesPushTokensByApplication(msg.ApplicationType)
                        .PipeTo(
                            this.Self,
                            success: result => new UpdateAvailableWithAdditionalData(result),
                            failure: err => new UpdateAvailableError(err));
                    break;

                case UpdateAvailableWithAdditionalData msg:
                    this.SendUpdateNotification(msg);
                    break;

                case UpdateAvailableError msg:
                    this.logger.Warning(msg.Exception.ToString());
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
            var pushNotification = this.CreatePushNotification(message.DevicesPushTokens);
            Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));
        }

        private PushNotification CreatePushNotification(IEnumerable<DevicePushToken> pushTokens)
        {
            var content = new PushNotificationContent
            {
                Title = "Update available",
                Body = "New application version is available.",
                CustomData = new
                {
                    Type = UpdateAvailablePushNotificationType
                }
            };

            return new PushNotification(content, pushTokens);
        }

        private class UpdateAvailableWithAdditionalData
        {
            public UpdateAvailableWithAdditionalData(IEnumerable<DevicePushToken> devicesPushTokens)
            {
                this.DevicesPushTokens = devicesPushTokens;
            }

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
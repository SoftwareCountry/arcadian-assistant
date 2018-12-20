namespace Arcadia.Assistant.Web.PushNotifications
{
    using Akka.Actor;

    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Models;

    public class PushNotificationsService : IPushNotificationsService
    {
        private readonly ActorSelection pushNotificationsDevicesActor;

        public PushNotificationsService(
            IActorRefFactory actorsFactory,
            ActorPathsBuilder actorPathsBuilder)
        {
            this.pushNotificationsDevicesActor = actorsFactory.ActorSelection(
                actorPathsBuilder.Get(WellKnownActorPaths.PushNotificationsDevices));
        }

        public void RegisterDevice(
            string employeeId,
            string deviceId,
            PushNotificationDeviceModel.DeviceTypeEnum deviceType)
        {
            var message = new RegisterPushNotificationsDevice(
                employeeId,
                deviceId,
                deviceType == PushNotificationDeviceModel.DeviceTypeEnum.Android
                    ? PushDeviceTypes.Android
                    : PushDeviceTypes.Ios);
            this.pushNotificationsDevicesActor.Tell(message);
        }

        public void RemoveDevice(string employeeId, string deviceId)
        {
            var message = new RemovePushNotificationsDevice(employeeId, deviceId);
            this.pushNotificationsDevicesActor.Tell(message);
        }
    }
}
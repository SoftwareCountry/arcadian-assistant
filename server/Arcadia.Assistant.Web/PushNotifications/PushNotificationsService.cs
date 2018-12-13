namespace Arcadia.Assistant.Web.PushNotifications
{
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    public class PushNotificationsService : IPushNotificationsService
    {
        private readonly ActorSelection pushNotificationsDevicesActor;
        private readonly ITimeoutSettings timeoutSettings;

        public PushNotificationsService(
            IActorRefFactory actorsFactory,
            ActorPathsBuilder actorPathsBuilder,
            ITimeoutSettings timeoutSettings)
        {
            this.pushNotificationsDevicesActor = actorsFactory.ActorSelection(
                actorPathsBuilder.Get(WellKnownActorPaths.PushNotificationsDevices));
            this.timeoutSettings = timeoutSettings;
        }

        public Task<RegisterPushNotificationsDevice.Response> RegisterDevice(string employeeId, string deviceId, CancellationToken cancellationToken)
        {
            var message = new RegisterPushNotificationsDevice(employeeId, deviceId);
            return this.pushNotificationsDevicesActor.Ask<RegisterPushNotificationsDevice.Response>(
                message,
                this.timeoutSettings.Timeout,
                cancellationToken);
        }

        public Task<RemovePushNotificationsDevice.Response> RemoveDevice(string employeeId, string deviceId, CancellationToken cancellationToken)
        {
            var message = new RemovePushNotificationsDevice(employeeId, deviceId);
            return this.pushNotificationsDevicesActor.Ask<RemovePushNotificationsDevice.Response>(
                message,
                this.timeoutSettings.Timeout,
                cancellationToken);
        }
    }
}
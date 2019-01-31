namespace Arcadia.Assistant.Notifications.Push
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Notifications.Push.Events;
    using Arcadia.Assistant.Patterns;

    public class PushNotificationsDevicesActor : UntypedPersistentActor, ILogReceive
    {
        private readonly Dictionary<string, HashSet<string>> deviceTokensByEmployeeId = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, string> deviceTypeByToken = new Dictionary<string, string>();

        public override string PersistenceId => "push-notifications-devices";

        public static Props CreateProps()
        {
            return Props.Create(() => new PushNotificationsDevicesActor());
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetDevicePushTokens msg:
                    this.GetDeviceTokens(msg);
                    break;

                case RegisterPushNotificationsDevice msg:
                    this.RegisterDevice(msg);
                    break;

                case RemovePushNotificationsDevice msg:
                    this.RemoveDevice(msg);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case EmployeeDeviceRegistered ev:
                    this.OnEmployeeDeviceRegistered(ev);
                    break;

                case EmployeeDeviceRemoved ev:
                    this.OnEmployeeDeviceRemoved(ev);
                    break;
            }
        }

        private void GetDeviceTokens(GetDevicePushTokens message)
        {
            this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceTokens);

            var devicePushTokens = deviceTokens?.Select(token => new DevicePushToken(token, this.deviceTypeByToken[token]));

            this.Sender.Tell(new GetDevicePushTokens.Success(devicePushTokens ?? Enumerable.Empty<DevicePushToken>()));
        }

        private void RegisterDevice(RegisterPushNotificationsDevice message)
        {
            if (!PushDeviceTypes.IsKnownType(message.DeviceType))
            {
                return;
            }

            var @event = new EmployeeDeviceRegistered(
                DateTimeOffset.Now,
                message.EmployeeId,
                message.DeviceId,
                message.DeviceType);

            this.Persist(@event, this.OnEmployeeDeviceRegistered);
        }

        private void RemoveDevice(RemovePushNotificationsDevice message)
        {
            if (!this.deviceTokensByEmployeeId.ContainsKey(message.EmployeeId))
            {
                return;
            }

            var @event = new EmployeeDeviceRemoved(
                DateTimeOffset.Now,
                message.EmployeeId,
                message.DeviceId);

            this.Persist(@event, this.OnEmployeeDeviceRemoved);

        }

        private void OnEmployeeDeviceRegistered(EmployeeDeviceRegistered @event)
        {
            if (!this.deviceTokensByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceTokens))
            {
                deviceTokens = new HashSet<string>();
                this.deviceTokensByEmployeeId.Add(@event.EmployeeId, deviceTokens);
            }

            deviceTokens.Add(@event.DeviceToken);
            this.deviceTypeByToken[@event.DeviceToken] = @event.DeviceType;
        }

        private void OnEmployeeDeviceRemoved(EmployeeDeviceRemoved @event)
        {
            if (!this.deviceTokensByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceTokens))
            {
                return;
            }

            deviceTokens.Remove(@event.DeviceToken);
            this.deviceTypeByToken.Remove(@event.DeviceToken);
        }
    }
}
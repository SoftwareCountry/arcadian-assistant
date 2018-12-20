namespace Arcadia.Assistant.Notifications.Push
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Notifications.Push.Events;

    public class PushNotificationsDevicesActor : UntypedPersistentActor, ILogReceive
    {
        private readonly Dictionary<string, HashSet<string>> deviceTokensByEmployeeId = new Dictionary<string, HashSet<string>>();

        // Multiple device types for one token to avoid very small chance of collisions between push tokens from different devices
        private readonly Dictionary<string, HashSet<string>> deviceTypesByToken = new Dictionary<string, HashSet<string>>();

        public override string PersistenceId => "push-notifications-devices";

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

            var devicePushTokens = deviceTokens?.SelectMany(token =>
            {
                var deviceTypes = this.deviceTypesByToken[token];
                return deviceTypes.Select(deviceType => new DevicePushToken(token, deviceType));
            });

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
            if (!this.deviceTokensByEmployeeId.ContainsKey(@event.EmployeeId))
            {
                this.deviceTokensByEmployeeId.Add(@event.EmployeeId, new HashSet<string>());
                this.deviceTypesByToken.Add(@event.DeviceToken, new HashSet<string>());
            }

            this.deviceTokensByEmployeeId[@event.DeviceToken].Add(@event.DeviceToken);
            this.deviceTypesByToken[@event.DeviceToken].Add(@event.DeviceType);
        }

        private void OnEmployeeDeviceRemoved(EmployeeDeviceRemoved @event)
        {
            if (!this.deviceTokensByEmployeeId.ContainsKey(@event.EmployeeId))
            {
                return;
            }

            this.deviceTokensByEmployeeId[@event.EmployeeId].Remove(@event.DeviceToken);

            // We don't know device type of specified token here, so we have to remove all device types for this token
            this.deviceTypesByToken[@event.DeviceToken].Clear();
        }
    }
}
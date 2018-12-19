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
        private readonly Dictionary<string, Dictionary<string, DevicePushToken>> deviceTokensByEmployeeId =
            new Dictionary<string, Dictionary<string, DevicePushToken>>();

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
            this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceIds);
            var devicePushTokens = deviceIds?.Values.ToList() ?? Enumerable.Empty<DevicePushToken>();
            this.Sender.Tell(new GetDevicePushTokens.Success(devicePushTokens));
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
            if (!this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceTokens) ||
                !deviceTokens.ContainsKey(message.DeviceId))
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
            if (!this.deviceTokensByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceIds))
            {
                deviceIds = new Dictionary<string, DevicePushToken>();
                this.deviceTokensByEmployeeId.Add(@event.EmployeeId, deviceIds);
            }

            if (!deviceIds.ContainsKey(@event.DeviceId))
            {
                deviceIds[@event.DeviceId] = new DevicePushToken(@event.DeviceId, @event.DeviceType);
            }
        }

        private void OnEmployeeDeviceRemoved(EmployeeDeviceRemoved @event)
        {
            if (this.deviceTokensByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceIds))
            {
                if (deviceIds.ContainsKey(@event.DeviceId))
                {
                    deviceIds.Remove(@event.DeviceId);
                }
            }
        }
    }
}
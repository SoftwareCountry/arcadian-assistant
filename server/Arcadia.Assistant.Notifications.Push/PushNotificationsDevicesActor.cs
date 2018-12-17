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
        private readonly Dictionary<string, HashSet<string>> deviceTokensByEmployeeId =
            new Dictionary<string, HashSet<string>>();

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
            this.Sender.Tell(new GetDevicePushTokens.Success(deviceIds?.ToList() ?? Enumerable.Empty<string>()));
        }

        private void RegisterDevice(RegisterPushNotificationsDevice message)
        {
            var @event = new EmployeeDeviceRegistered(
                DateTimeOffset.Now,
                message.EmployeeId,
                message.DeviceId);

            this.Persist(@event, this.OnEmployeeDeviceRegistered);
        }

        private void RemoveDevice(RemovePushNotificationsDevice message)
        {
            if (!this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceTokens) || 
                !deviceTokens.Contains(message.DeviceId))
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
                deviceIds = new HashSet<string>();
                this.deviceTokensByEmployeeId.Add(@event.EmployeeId, deviceIds);
            }

            if (!deviceIds.Contains(@event.DeviceId))
            {
                deviceIds.Add(@event.DeviceId);
            }
        }

        private void OnEmployeeDeviceRemoved(EmployeeDeviceRemoved @event)
        {
            if (this.deviceTokensByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceIds))
            {
                deviceIds.Remove(@event.DeviceId);
            }
        }
    }
}
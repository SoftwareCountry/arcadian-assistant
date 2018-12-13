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
        private readonly Dictionary<string, HashSet<string>> devicesByEmployeeId = new Dictionary<string, HashSet<string>>();

        public override string PersistenceId => "push-notifications-devices";

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetDeviceIds msg:
                    this.GetDeviceIds(msg);
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

        private void GetDeviceIds(GetDeviceIds message)
        {
            this.devicesByEmployeeId.TryGetValue(message.EmployeeId, out var deviceIds);
            this.Sender.Tell(new GetDeviceIds.Success(deviceIds ?? Enumerable.Empty<string>()));
        }

        private void RegisterDevice(RegisterPushNotificationsDevice message)
        {
            var @event = new EmployeeDeviceRegistered(
                DateTimeOffset.Now,
                message.EmployeeId,
                message.DeviceId);

            this.Persist(@event, ev =>
            {
                this.OnEmployeeDeviceRegistered(@event);
                this.Sender.Tell(RegisterPushNotificationsDevice.Success.Instance);
            });
        }

        private void RemoveDevice(RemovePushNotificationsDevice message)
        {
            if (!this.devicesByEmployeeId.TryGetValue(message.EmployeeId, out var deviceIds) || !deviceIds.Contains(message.DeviceId))
            {
                this.Sender.Tell(RemovePushNotificationsDevice.NotFoundError.Instance);
                return;
            }

            var @event = new EmployeeDeviceRemoved(
                DateTimeOffset.Now,
                message.EmployeeId,
                message.DeviceId);

            this.Persist(@event, ev =>
            {
                this.OnEmployeeDeviceRemoved(ev);
                this.Sender.Tell(RemovePushNotificationsDevice.Success.Instance);
            });

        }

        private void OnEmployeeDeviceRegistered(EmployeeDeviceRegistered @event)
        {
            if (!this.devicesByEmployeeId.TryGetValue(@event.EmployeeId, out var deviceIds))
            {
                deviceIds = new HashSet<string>();
                this.devicesByEmployeeId.Add(@event.EmployeeId, deviceIds);
            }

            if (!deviceIds.Contains(@event.DeviceId))
            {
                deviceIds.Add(@event.DeviceId);
            }
        }

        private void OnEmployeeDeviceRemoved(EmployeeDeviceRemoved @event)
        {
            var deviceIds = this.devicesByEmployeeId[@event.EmployeeId];
            deviceIds.Remove(@event.DeviceId);
        }
    }
}
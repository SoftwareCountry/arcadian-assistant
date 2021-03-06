﻿namespace Arcadia.Assistant.Notifications.Push
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
                case GetDevicePushTokensByEmployee msg:
                    this.GetDeviceTokens(msg);
                    break;

                case GetDevicePushTokensByApplication msg:
                    this.GetDeviceTokensByApplication(msg);
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

        private void GetDeviceTokens(GetDevicePushTokensByEmployee message)
        {
            this.deviceTokensByEmployeeId.TryGetValue(message.EmployeeId, out var deviceTokens);

            var devicePushTokens = deviceTokens?.Select(token => new DevicePushToken(token, this.deviceTypeByToken[token]));

            this.Sender.Tell(new GetDevicePushTokensByEmployee.Success(devicePushTokens ?? Enumerable.Empty<DevicePushToken>()));
        }

        private void GetDeviceTokensByApplication(GetDevicePushTokensByApplication message)
        {
            var allDeviceTokens = this.deviceTokensByEmployeeId.Values.SelectMany(x => x);

            var applicationDeviceTokens = allDeviceTokens
                .Where(token => this.deviceTypeByToken[token] == message.DeviceType)
                .Select(token => new DevicePushToken(token, message.DeviceType))
                .ToArray();

            this.Sender.Tell(new GetDevicePushTokensByApplication.Response(applicationDeviceTokens));
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
            this.RemoveDeviceTokenFromPreviousEmployees(@event.DeviceToken);

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

        private void RemoveDeviceTokenFromPreviousEmployees(string deviceToken)
        {
            var employeesTokens = this.deviceTokensByEmployeeId.Values.ToList();
            foreach (var employeeTokens in employeesTokens)
            {
                employeeTokens.Remove(deviceToken);
            }
        }
    }
}
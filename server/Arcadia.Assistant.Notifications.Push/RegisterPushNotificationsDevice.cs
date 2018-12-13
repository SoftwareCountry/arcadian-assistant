﻿namespace Arcadia.Assistant.Notifications.Push
{
    public class RegisterPushNotificationsDevice
    {
        public RegisterPushNotificationsDevice(string employeeId, string deviceId)
        {
            this.EmployeeId = employeeId;
            this.DeviceId = deviceId;
        }

        public string EmployeeId { get; }

        public string DeviceId { get; }
    }
}
namespace Arcadia.Assistant.Notifications.Push.Events
{
    using System;

    public class EmployeeDeviceRegistered
    {
        public EmployeeDeviceRegistered(
            DateTimeOffset timestamp,
            string employeeId,
            string deviceToken,
            string deviceType)
        {
            this.Timestamp = timestamp;
            this.EmployeeId = employeeId;
            this.DeviceToken = deviceToken;
            this.DeviceType = deviceType;
        }

        public DateTimeOffset Timestamp { get; }

        public string EmployeeId { get; }

        public string DeviceToken { get; }

        public string DeviceType { get; }
    }
}
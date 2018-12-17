namespace Arcadia.Assistant.Notifications.Push.Events
{
    using System;

    public class EmployeeDeviceRegistered
    {
        public EmployeeDeviceRegistered(DateTimeOffset timestamp, string employeeId, string deviceId)
        {
            this.Timestamp = timestamp;
            this.EmployeeId = employeeId;
            this.DeviceId = deviceId;
        }

        public DateTimeOffset Timestamp { get; }

        public string EmployeeId { get; }

        public string DeviceId { get; }
    }
}
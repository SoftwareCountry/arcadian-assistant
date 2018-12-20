namespace Arcadia.Assistant.Notifications.Push.Events
{
    using System;

    public class EmployeeDeviceRemoved
    {
        public EmployeeDeviceRemoved(DateTimeOffset timestamp, string employeeId, string deviceToken)
        {
            this.Timestamp = timestamp;
            this.EmployeeId = employeeId;
            this.DeviceToken = deviceToken;
        }

        public DateTimeOffset Timestamp { get;  }

        public string EmployeeId { get; }

        public string DeviceToken { get;  }
    }
}
namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator.Contracts.Models
{
    public class RemovePushNotificationsDevice
    {
        public RemovePushNotificationsDevice(string employeeId, string deviceId)
        {
            this.EmployeeId = employeeId;
            this.DeviceId = deviceId;
        }

        public string EmployeeId { get; }

        public string DeviceId { get; }
    }
}
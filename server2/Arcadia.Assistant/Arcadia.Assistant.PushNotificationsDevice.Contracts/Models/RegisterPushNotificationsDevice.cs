namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator.Contracts.Models
{
    public class RegisterPushNotificationsDevice
    {
        public RegisterPushNotificationsDevice(string employeeId, string deviceId, string deviceType)
        {
            this.EmployeeId = employeeId;
            this.DeviceId = deviceId;
            this.DeviceType = deviceType;
        }

        public string EmployeeId { get; }

        public string DeviceId { get; }

        public string DeviceType { get; }
    }
}
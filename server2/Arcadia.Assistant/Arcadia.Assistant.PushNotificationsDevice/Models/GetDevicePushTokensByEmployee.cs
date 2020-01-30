namespace Arcadia.Assistant.PushNotification.Models
{
    using System.Collections.Generic;

    using PushNotificationsDeviceRegistrator.Contracts.Models;

    public class GetDevicePushTokensByEmployee
    {
        public GetDevicePushTokensByEmployee(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(IEnumerable<DevicePushToken> devicePushTokens)
            {
                this.DevicePushTokens = devicePushTokens;
            }

            public IEnumerable<DevicePushToken> DevicePushTokens { get; }
        }
    }
}
namespace Arcadia.Assistant.Notifications.Push
{
    using System.Collections.Generic;

    public class GetDevicePushTokens
    {
        public GetDevicePushTokens(string employeeId)
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
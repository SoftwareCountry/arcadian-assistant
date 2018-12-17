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
            public Success(IEnumerable<string> devicePushTokens)
            {
                this.DevicePushTokens = devicePushTokens;
            }

            public IEnumerable<string> DevicePushTokens { get; }
        }
    }
}
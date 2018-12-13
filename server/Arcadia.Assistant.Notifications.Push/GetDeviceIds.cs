namespace Arcadia.Assistant.Notifications.Push
{
    using System.Collections.Generic;

    public class GetDeviceIds
    {
        public GetDeviceIds(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(IEnumerable<string> deviceIds)
            {
                this.DeviceIds = deviceIds;
            }

            public IEnumerable<string> DeviceIds { get; }
        }
    }
}
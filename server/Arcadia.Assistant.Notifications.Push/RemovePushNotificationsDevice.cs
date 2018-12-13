namespace Arcadia.Assistant.Notifications.Push
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

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public static readonly Success Instance = new Success();
        }

        public class NotFoundError : Response
        {
            public static readonly NotFoundError Instance = new NotFoundError();
        }
    }
}
namespace Arcadia.Assistant.Web.PushNotifications
{
    public interface IPushNotificationsService
    {
        void RegisterDevice(string employeeId, string deviceId);

        void RemoveDevice(string employeeId, string deviceId);
    }
}
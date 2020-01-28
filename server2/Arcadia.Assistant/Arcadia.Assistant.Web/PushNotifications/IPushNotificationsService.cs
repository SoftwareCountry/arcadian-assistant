namespace Arcadia.Assistant.Web.PushNotifications
{
    using Arcadia.Assistant.Web.Models;

    public interface IPushNotificationsService
    {
        void RegisterDevice(
            string employeeId,
            string deviceId,
            DeviceType deviceType);

        void RemoveDevice(string employeeId, string deviceId);
    }
}
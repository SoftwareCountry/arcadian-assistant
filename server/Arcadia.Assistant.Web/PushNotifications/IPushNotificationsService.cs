namespace Arcadia.Assistant.Web.PushNotifications
{
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Notifications.Push;

    public interface IPushNotificationsService
    {
        Task<RegisterPushNotificationsDevice.Response> RegisterDevice(string employeeId, string deviceId, CancellationToken cancellationToken);

        Task<RemovePushNotificationsDevice.Response> RemoveDevice(string employeeId, string deviceId, CancellationToken cancellationToken);
    }
}
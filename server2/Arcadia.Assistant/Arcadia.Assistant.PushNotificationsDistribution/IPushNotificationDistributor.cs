namespace Arcadia.Assistant.PushNotificationsDistributor
{
    using System.Threading.Tasks;

    using Contracts.Models;

    public interface IPushNotificationDistributor
    {
        Task SendPushNotification(PushNotificationMessage message);
    }
}
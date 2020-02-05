namespace Arcadia.Assistant.Notifications
{
    using DeviceRegistry.Contracts.Models;

    using PushNotifications.Contracts.Models;

    public static class Extensions
    {
        public static DeviceToken ToDeviceToken(this DeviceRegistryItem item) =>
            new DeviceToken()
            {
                DeviceId = item.DeviceId,
                DeviceType = item.DeviceType
            };
    }
}
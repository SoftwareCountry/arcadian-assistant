namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts.Models
{
    using System.Collections.Generic;

    using PushNotificationsDeviceRegistrator.Contracts.Models;

    public class PushNotificationMessage
    {
        public PushNotificationMessage(
            PushNotificationContent content,
            IEnumerable<DevicePushToken> devicePushTokens)
        {
            this.Content = content;
            this.DevicePushTokens = devicePushTokens;
        }

        public PushNotificationContent Content { get; }

        public IEnumerable<DevicePushToken> DevicePushTokens { get; }
    }
}
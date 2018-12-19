namespace Arcadia.Assistant.Notifications.Push
{
    using System.Collections.Generic;

    public class PushNotification
    {
        public PushNotification(
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
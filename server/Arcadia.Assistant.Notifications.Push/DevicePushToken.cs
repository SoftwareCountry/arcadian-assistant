namespace Arcadia.Assistant.Notifications.Push
{
    using System.Collections.Generic;

    public class DevicePushToken
    {
        public DevicePushToken(string token, string deviceType)
        {
            this.Token = token;
            this.DeviceType = deviceType;
        }

        public string Token { get; }

        public string DeviceType { get; }
    }
}
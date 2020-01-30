namespace Arcadia.Assistant.PushNotification.Models

{
    using System.Collections.Generic;

    using PushNotificationsDeviceRegistrator.Contracts.Models;

    public class GetDevicePushTokensByApplication
    {
        public GetDevicePushTokensByApplication(string deviceType)
        {
            this.DeviceType = deviceType;
        }

        public string DeviceType { get; }

        public class Response
        {
            public Response(IEnumerable<DevicePushToken> devicePushTokens)
            {
                this.DevicePushTokens = devicePushTokens;
            }

            public IEnumerable<DevicePushToken> DevicePushTokens { get; }
        }
    }
}
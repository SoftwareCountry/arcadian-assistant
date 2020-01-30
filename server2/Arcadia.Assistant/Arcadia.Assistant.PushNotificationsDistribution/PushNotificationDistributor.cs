namespace Arcadia.Assistant.PushNotificationsDistributor
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Contracts.Models;

    public class PushNotificationDistributor : IPushNotificationDistributor
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPushSettings pushSettings;

        public PushNotificationDistributor(IPushSettings pushSettings, IHttpClientFactory httpClientFactory)
        {
            this.pushSettings = pushSettings;
            this.httpClientFactory = httpClientFactory;
        }

        Task IPushNotificationDistributor.SendPushNotification(PushNotificationMessage message)
        {
            //this.logger.Debug("Push notification message received");

            if (!message.DevicePushTokens.Any())
            {
                //this.logger.Debug("Push notification message doesn't contain target devices and won't be sent");
                return Task.CompletedTask;
            }

            var androidPushTask = this.SendApplicationPushNotification(message, WellKnownBuildTypes.Android, this.pushSettings.AndroidPushUrl);
            var iosPushTask = this.SendApplicationPushNotification(message, WellKnownBuildTypes.Ios, this.pushSettings.IosPushUrl);

            return Task.WhenAll(androidPushTask, iosPushTask);
        }

        private Task SendApplicationPushNotification(PushNotificationMessage message, string deviceType, string pushUrl)
        {
            var pushNotificationPayload = new PushNotificationPayload
            {
                Content = message.Content,
                Target = new PushNotificationTarget
                {
                    DevicePushTokens = message.DevicePushTokens
                        .Where(x => x.DeviceType == deviceType)
                        .Select(x => x.Token)
                        .ToList()
                }
            };

            if (!pushNotificationPayload.Target.DevicePushTokens.Any())
            {
                return Task.CompletedTask;
            }

            var jsonMessage = this.SerializeNotification(pushNotificationPayload);
            //this.logger.Debug($"Serialized {deviceType} notification message: {jsonMessage}");

            return this.SendPushNotificationRequest(pushUrl, jsonMessage);
        }

        private async Task SendPushNotificationRequest(
            string applicationPushUrl,
            string pushNotificationContent)
        {
            var content = new StringContent(pushNotificationContent);
            content.Headers.ContentType.MediaType = "application/json";

            var request = new HttpRequestMessage(HttpMethod.Post, applicationPushUrl)
            {
                Content = content
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            content.Headers.Add("X-API-Token", this.pushSettings.ApiToken);

            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        //this.logger.Debug($"Push notification was successfully sent to {response.RequestMessage.RequestUri}");
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        //this.logger.Warning($"Push notification to {response.RequestMessage.RequestUri} failed: {responseContent}");
                    }
                }
            }
        }

        private string SerializeNotification(PushNotificationPayload message)
        {
            var serializerSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Serialize(message, serializerSettings);
        }

        /*
        private class SendPushNotificationResultFinish
        {
            public static readonly SendPushNotificationResultFinish Instance = new SendPushNotificationResultFinish();

            public SendPushNotificationResultFinish(string errorMessage = null)
            {
                this.ErrorMessage = errorMessage;
            }

            public string ErrorMessage { get; }
        }
        */
    }
}
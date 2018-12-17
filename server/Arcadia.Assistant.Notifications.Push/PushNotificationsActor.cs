namespace Arcadia.Assistant.Notifications.Push
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Arcadia.Assistant.Configuration.Configuration;

    public class PushNotificationsActor : UntypedActor, ILogReceive
    {
        private readonly IPushSettings pushSettings;
        private readonly IHttpClientFactory httpClientFactory;

        public readonly ILoggingAdapter logger = Context.GetLogger();

        public PushNotificationsActor(IPushSettings pushSettings, IHttpClientFactory httpClientFactory)
        {
            this.pushSettings = pushSettings;
            this.httpClientFactory = httpClientFactory;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case PushNotification msg when this.pushSettings.Enabled:
                    this.SendPushNotification(msg)
                        .PipeTo(
                            this.Self,
                            success: () => SendPushNotificationResultFinish.Instance,
                            failure: err => new SendPushNotificationResultFinish(err.Message));
                    break;

                case SendPushNotificationResultFinish msg:
                    if (msg.ErrorMessage != null)
                    {
                        this.logger.Warning(msg.ErrorMessage);
                    }

                    break;
            }
        }

        private Task SendPushNotification(PushNotification message)
        {
            this.logger.Debug("Push notification message received");

            var jsonMessage = this.SerializeNotification(message);
            this.logger.Debug($"Serialized notification message: {jsonMessage}");

            var requests = this.pushSettings.ApplicationPushUrls
                .Select(url => this.SendPushNotificationRequest(url, jsonMessage));

            return Task.WhenAll(requests);
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
                        this.logger.Debug($"Push notification was succesfully sent to {response.RequestMessage.RequestUri}");
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        this.logger.Warning($"Push notification to {response.RequestMessage.RequestUri} failed: {responseContent}");
                    }
                }
            }
        }

        private string SerializeNotification(PushNotification message)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(message, serializerSettings);
        }

        private class SendPushNotificationResultFinish
        {
            public static readonly SendPushNotificationResultFinish Instance = new SendPushNotificationResultFinish();

            public SendPushNotificationResultFinish(string errorMessage = null)
            {
                this.ErrorMessage = errorMessage;
            }

            public string ErrorMessage { get; }
        }
    }
}
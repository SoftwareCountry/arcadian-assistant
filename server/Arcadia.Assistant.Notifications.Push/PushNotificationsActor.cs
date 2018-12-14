namespace Arcadia.Assistant.Notifications.Push
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;

    using Akka.Actor;
    using Akka.Event;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Arcadia.Assistant.Configuration.Configuration;

    public class PushNotificationsActor : UntypedActor, ILogReceive
    {
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

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
                    this.SendPushNotification(msg);
                    break;
            }
        }

        private void SendPushNotification(PushNotification message)
        {
            this.logger.Debug("Push notification message received");

            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                var jsonMessage = this.SerializeNotification(message);
                this.logger.Debug($"Serialized notification message: {jsonMessage}");

                var content = new StringContent(jsonMessage);
                content.Headers.ContentType.MediaType = "application/json";

                var request = new HttpRequestMessage(HttpMethod.Post, this.pushSettings.ApplicationPushUrl)
                {
                    Content = content
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                content.Headers.Add("X-API-Token", this.pushSettings.ApiToken);

                var cancellationSource = new CancellationTokenSource(this.timeout);
                var requestTask = httpClient.SendAsync(request, cancellationSource.Token);
                requestTask.Wait(CancellationToken.None);

                if (requestTask.Result.StatusCode == HttpStatusCode.Accepted)
                {
                    this.logger.Debug("Push notification was succesfully sent");
                }
                else
                {
                    var contentToStringTask = requestTask.Result.Content.ReadAsStringAsync();
                    contentToStringTask.Wait(CancellationToken.None);
                    this.logger.Warning($"Push notification failed: {contentToStringTask.Result}");
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
    }
}
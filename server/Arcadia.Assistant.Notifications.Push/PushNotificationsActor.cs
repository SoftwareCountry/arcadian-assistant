namespace Arcadia.Assistant.Notifications.Push
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

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

            var jsonMessage = this.SerializeNotification(message);
            this.logger.Debug($"Serialized notification message: {jsonMessage}");

            var cancellationSource = new CancellationTokenSource(this.timeout);

            var requests = this.pushSettings.ApplicationPushUrls
                .Select(url => this.SendPushNotificationRequest(jsonMessage, url, cancellationSource.Token))
                .ToArray();

            Task.WaitAll(requests, CancellationToken.None);

            var failedRequestsResults = requests
                .Where(r => r.Result.StatusCode != HttpStatusCode.Accepted)
                .Select(r => r.Result)
                .ToList();

            if (failedRequestsResults.Count == 0)
            {
                this.logger.Debug("Push notification was succesfully sent");
                return;
            }

            foreach (var result in failedRequestsResults)
            {
                var contentToStringTask = result.Content.ReadAsStringAsync();
                contentToStringTask.Wait(CancellationToken.None);
                this.logger.Warning($"Push notification to {result.RequestMessage.RequestUri} failed: {contentToStringTask.Result}");
            }
        }

        private Task<HttpResponseMessage> SendPushNotificationRequest(
            string pushNotificationContent,
            string applicationPushUrl,
            CancellationToken cancellationToken)
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
                return httpClient.SendAsync(request, cancellationToken);
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
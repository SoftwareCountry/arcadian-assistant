namespace Arcadia.Assistant.PushNotifications
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

    using DeviceRegistry.Contracts.Models;

    using Interfaces;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Models;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class PushNotifications : StatelessService, IPushNotifications
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger logger;
        private readonly Dictionary<string, string> notificationConfiguration;
        private readonly IPushSettings pushSettings;

        public PushNotifications(
            StatelessServiceContext context, IPushSettings pushSettings, IHttpClientFactory httpClientFactory,
            ILogger<PushNotifications> logger)
            : base(context)
        {
            this.pushSettings = pushSettings;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;

            this.notificationConfiguration = new Dictionary<string, string>
            {
                { WellKnownDeviceTypes.Android, this.pushSettings.AndroidPushUrl },
                { WellKnownDeviceTypes.Ios, this.pushSettings.IosPushUrl }
            };
        }

        public async Task SendPushNotification(
            DeviceRegistryEntry[] deviceTokens,
            PushNotificationContent notificationContent,
            CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Push notification message received");

            var deviceInfos = deviceTokens
                .GroupBy(x => x.DeviceType)
                .ToDictionary(x => x.Key.Value, x => x.Select(g => g));
            if (!deviceInfos.Any())
            {
                this.logger.LogDebug("Push notification message doesn't contain target devices and won't be sent");
                return;
            }

            // send notifications for registered device types
            foreach (var key in this.notificationConfiguration.Keys)
            {
                if (deviceInfos.TryGetValue(key, out var entry))
                {
                    await this.SendApplicationPushNotification(
                        entry
                            .Select(x => x.DeviceId.ToString())
                            .ToList()
                            .AsReadOnly(),
                        notificationContent, this.notificationConfiguration[key]);
                }
                else
                {
                    this.logger.LogDebug("Device type {DeviceType} has no registrations for notifications", key);
                }
            }
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private async Task SendApplicationPushNotification(
            IReadOnlyCollection<string> deviceIds, PushNotificationContent notificationContent, string pushUrl)
        {
            if (!deviceIds.Any())
            {
                this.logger.LogWarning("Push notification decline: device tokens list is empty.");
                return;
            }

            var pushNotificationPayload = new PushNotificationPayload
            {
                Content = notificationContent,
                Target = new PushNotificationTarget
                {
                    DevicePushTokens = deviceIds
                }
            };

            try
            {
                var jsonMessage = this.SerializeNotification(pushNotificationPayload);
                this.logger.LogDebug("Serialized notification message: {JsonMessage}", jsonMessage);

                await this.SendPushNotificationRequest(pushUrl, jsonMessage);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Send push notification for {DeviceCount} devices.", deviceIds.Count);
            }
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

            using var httpClient = this.httpClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                this.logger.LogDebug("Push notification was successfully sent to '{RequestUri}'",
                    response.RequestMessage.RequestUri);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                this.logger.LogWarning("Push notification to '{RequestUri}' failed: {ResponseContent}",
                    response.RequestMessage.RequestUri, responseContent);
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
    }
}
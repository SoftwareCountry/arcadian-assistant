using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.PushNotifications.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.PushNotifications
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security;
    using System.Text.Json;

    using Castle.Core.Internal;

    using Contracts;
    using Contracts.Models;

    using Models;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public sealed class PushNotifications : StatelessService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPushSettings pushSettings;
        private readonly Dictionary<string, string> notificationConfiguration;

        public PushNotifications(StatelessServiceContext context, IPushSettings pushSettings, IHttpClientFactory httpClientFactory)
            : base(context)
        {
            this.pushSettings = pushSettings;
            this.httpClientFactory = httpClientFactory;

            notificationConfiguration = new Dictionary<string, string>()
            {
                { WellKnownBuildTypes.Android, this.pushSettings.AndroidPushUrl },
                { WellKnownBuildTypes.Ios, this.pushSettings.IosPushUrl },
            };
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        public async Task SendPushNotification(IEnumerable<DeviceToken> deviceTokens, PushNotificationContent notificationContent, CancellationToken cancellationToken)
        {
            //this.logger.Debug("Push notification message received");

            if (deviceTokens.IsNullOrEmpty())
            {
                //this.logger.Debug("Push notification message doesn't contain target devices and won't be sent");
                return;
            }

            // send notifications for registered device types
            foreach (var key in notificationConfiguration.Keys)
            {
                await this.SendApplicationPushNotification(
                    deviceTokens.Where(x => x.DeviceType == key),
                    notificationContent,
                    notificationConfiguration[key]);
            }
        }

        private async Task SendApplicationPushNotification(IEnumerable<DeviceToken> deviceTokens, PushNotificationContent notificationContent, string pushUrl)
        {
            if (deviceTokens.IsNullOrEmpty())
            {
                return;
            }

            var pushNotificationPayload = new PushNotificationPayload
            {
                Content = notificationContent,
                Target = new PushNotificationTarget
                {
                    DevicePushTokens = deviceTokens
                        .Select(x => x.DeviceId)
                        .ToList()
                }
            };

            var jsonMessage = this.SerializeNotification(pushNotificationPayload);
            //this.logger.Debug($"Serialized {deviceType} notification message: {jsonMessage}");

            await this.SendPushNotificationRequest(pushUrl, jsonMessage);
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
                //this.logger.Debug($"Push notification was successfully sent to {response.RequestMessage.RequestUri}");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                //this.logger.Warning($"Push notification to {response.RequestMessage.RequestUri} failed: {responseContent}");
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

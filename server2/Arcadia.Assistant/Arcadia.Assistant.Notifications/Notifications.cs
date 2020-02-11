namespace Arcadia.Assistant.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Castle.Core.Internal;

    using Contracts;
    using Contracts.Models;

    using DeviceRegistry.Contracts;

    using Interfaces;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Models;

    using PushNotifications.Contracts;
    using PushNotifications.Contracts.Models;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public sealed class Notifications : StatelessService, INotifications
    {
        private readonly IDeviceRegistry deviceRegistry;
        private readonly ILogger logger;

        private readonly Dictionary<CalendarEventType, IEnumerable<NotificationProviderType>> notificationProvidersMap;
        private readonly INotificationSettings notificationSettings;
        private readonly IPushNotifications pushNotifications;

        public Notifications(
            StatelessServiceContext context,
            INotificationSettings notificationSettings,
            IDeviceRegistry deviceRegistry,
            IPushNotifications pushNotifications,
            ILogger logger)
            : base(context)
        {
            this.notificationSettings = notificationSettings;
            this.deviceRegistry = deviceRegistry;
            this.pushNotifications = pushNotifications;
            this.logger = logger;
            this.notificationProvidersMap = new Dictionary<CalendarEventType, IEnumerable<NotificationProviderType>>
            {
                {
                    CalendarEventType.Vacation, new List<NotificationProviderType>
                        { NotificationProviderType.Push }
                },
                {
                    CalendarEventType.Dayoff, new List<NotificationProviderType>
                        { NotificationProviderType.Push }
                },
                {
                    CalendarEventType.Workout, new List<NotificationProviderType>
                        { NotificationProviderType.Push }
                },
                {
                    CalendarEventType.Sickleave, new List<NotificationProviderType>
                        { NotificationProviderType.Push }
                }
            };
        }

        public async Task Send(IEnumerable<string> employeeIds, NotificationType notificationType, CalendarEventType eventType, NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            if (employeeIds.IsNullOrEmpty() || !this.notificationProvidersMap.ContainsKey(eventType))
            {
                return;
            }

            var notificationProviders = this.notificationProvidersMap[eventType];
            foreach (var providerType in notificationProviders)
            {
                switch (providerType)
                {
                    case NotificationProviderType.Push:
                        if (this.notificationSettings.EnablePush)
                        {
                            var tokens = await this.GetDeviceTokens(employeeIds, cancellationToken);
                            await this.SendPushNotification(tokens, notificationType, eventType, notificationMessage, cancellationToken);
                        }

                        break;
                }
            }
        }

        private async Task SendPushNotification(Dictionary<string, IEnumerable<DeviceToken>> deviceTokens, NotificationType notificationType, CalendarEventType eventType, NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            var notificationContent = this.CreatePushNotificationContent(notificationType, eventType, notificationMessage);
            foreach (var empl in deviceTokens.Keys)
            {
                await this.pushNotifications.SendPushNotification(deviceTokens[empl], notificationContent, cancellationToken);
            }
        }

        private async Task<Dictionary<string, IEnumerable<DeviceToken>>> GetDeviceTokens(IEnumerable<string> employeeIds, CancellationToken cancellationToken)
        {
            var tokens = await this.deviceRegistry.GetDeviceRegistryByEmployeeList(employeeIds, cancellationToken);
            return tokens.Keys.SelectMany(k => tokens[k].Select(x =>
                    new
                    {
                        Key = k,
                        Val = x.ToDeviceToken()
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(a => a.Val));
        }

        private PushNotificationContent CreatePushNotificationContent(NotificationType notificationType, CalendarEventType eventType, NotificationMessage notificationMessage)
        {
            var operation = notificationType == NotificationType.CalendarEventAdd ? "add" : notificationType == NotificationType.CalendarEventChange ? "change" : "delete";
            return new PushNotificationContent
            {
                Title = $"Push '{eventType}' notification for {operation}.",
                Body = notificationMessage.Text
            };
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.logger.LogInformation("Run notification iteration");

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
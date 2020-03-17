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
    using DeviceRegistry.Contracts.Models;

    using Employees.Contracts;

    using Interfaces;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using PushNotifications.Contracts;
    using PushNotifications.Contracts.Models;

    using UserPreferences.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Notifications : StatelessService, INotifications
    {
        private readonly IDeviceRegistry deviceRegistry;
        private readonly ILogger logger;

        private readonly Dictionary<string, IEnumerable<NotificationType>> notificationProvidersMap;
        private readonly INotificationSettings notificationSettings;
        private readonly IPushNotifications pushNotifications;
        private readonly IUsersPreferencesStorage userPreferences;

        public Notifications(
            StatelessServiceContext context,
            INotificationSettings notificationSettings,
            IDeviceRegistry deviceRegistry,
            IUsersPreferencesStorage userPreferences,
            IPushNotifications pushNotifications,
            ILogger<Notifications> logger)
            : base(context)
        {
            this.notificationSettings = notificationSettings;
            this.deviceRegistry = deviceRegistry;
            this.userPreferences = userPreferences;
            this.pushNotifications = pushNotifications;
            this.logger = logger;
            this.notificationProvidersMap = this.notificationSettings.ClientNotificationProvidersMap;
        }

        public async Task Send(IEnumerable<EmployeeId> employeeId, NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            if (employeeId.IsNullOrEmpty())
            {
                return;
            }

            var notificationProviders = this.notificationProvidersMap.TryGetValue(notificationMessage.ClientName, out var map) ? map : Enum.GetValues(typeof(NotificationType)).OfType<NotificationType>().ToList();
            foreach (var providerType in notificationProviders)
            {
                switch (providerType)
                {
                    case NotificationType.Push:
                        if (this.notificationSettings.EnablePush)
                        {
                            var tokens = await this.GetDeviceTokens(employeeId, cancellationToken);
                            await this.SendPushNotification(tokens, notificationMessage, cancellationToken);
                        }

                        break;
                }
            }
        }

        private async Task SendPushNotification(Dictionary<EmployeeId, IEnumerable<DeviceRegistryEntry>> deviceTokens, NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            var notificationContent = new PushNotificationContent
            {
                Title = notificationMessage.Subject,
                Body = notificationMessage.ShortText
            };

            foreach (var employeeId in deviceTokens.Keys)
            {
                await this.pushNotifications.SendPushNotification(deviceTokens[employeeId], notificationContent, cancellationToken);
            }
        }

        private async Task<Tuple<EmployeeId, UserPreferences>> GetUserPreferences(EmployeeId id, CancellationToken cancellationToken)
        {
            var prefs = await this.userPreferences.ForEmployee(id).Get(cancellationToken);
            return new Tuple<EmployeeId, UserPreferences>(id, prefs);
        }

        private async Task<Dictionary<EmployeeId, IEnumerable<DeviceRegistryEntry>>> GetDeviceTokens(IEnumerable<EmployeeId> employeeIds, CancellationToken cancellationToken)
        {
            var employeePreferences = (await Task.WhenAll(employeeIds
                    .Distinct()
                    .Select(x => this.GetUserPreferences(x, cancellationToken))))
                .ToDictionary(x => x.Item1, x => x.Item2);
            var activeEmployees = employeeIds.Where(x => employeePreferences.TryGetValue(x, out var pref) ? pref.PushNotifications : false);
            var tokens = await this.deviceRegistry.GetDeviceRegistryByEmployeeList(activeEmployees, cancellationToken);
            return tokens.Keys.SelectMany(k => tokens[k].Select(x =>
                    new
                    {
                        Key = k,
                        Val = x
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(a => a.Val));
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
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
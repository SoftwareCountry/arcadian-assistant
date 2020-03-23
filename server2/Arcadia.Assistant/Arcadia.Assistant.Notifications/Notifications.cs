namespace Arcadia.Assistant.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

    using DeviceRegistry.Contracts;
    using DeviceRegistry.Contracts.Models;

    using EmailNotifications.Contracts;
    using EmailNotifications.Contracts.Models;

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
        private readonly IEmailNotifications emailNotifications;
        private readonly IEmployees employees;
        private readonly ILogger logger;

        private readonly IReadOnlyDictionary<string, IReadOnlyCollection<NotificationType>> notificationProvidersMap;
        private readonly INotificationSettings notificationSettings;
        private readonly IPushNotifications pushNotifications;
        private readonly IUsersPreferencesStorage userPreferences;

        public Notifications(
            StatelessServiceContext context,
            INotificationSettings notificationSettings,
            IEmployees employees,
            IDeviceRegistry deviceRegistry,
            IUsersPreferencesStorage userPreferences,
            IPushNotifications pushNotifications,
            IEmailNotifications emailNotifications,
            ILogger<Notifications> logger)
            : base(context)
        {
            this.notificationSettings = notificationSettings;
            this.deviceRegistry = deviceRegistry;
            this.employees = employees;
            this.userPreferences = userPreferences;
            this.pushNotifications = pushNotifications;
            this.emailNotifications = emailNotifications;
            this.logger = logger;
            this.notificationProvidersMap = this.notificationSettings.NotificationTemplateProvidersMap;
        }

        private IReadOnlyCollection<NotificationType> AllNotificationTypes { get; } =
            Enum.GetValues(typeof(NotificationType)).OfType<NotificationType>().ToList().AsReadOnly();

        public async Task Send(
            EmployeeId[] employeeIds,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken)
        {
            if (!employeeIds.Any())
            {
                return;
            }

            var notificationProviders =
                this.notificationProvidersMap.TryGetValue(notificationMessage.NotificationTemplate, out var map)
                    ? map
                    : this.AllNotificationTypes;
            foreach (var providerType in notificationProviders)
            {
                switch (providerType)
                {
                    case NotificationType.Push:
                        if (this.notificationSettings.EnablePush)
                        {
                            var tokens = await this.GetDeviceTokens(employeeIds, cancellationToken);
                            var customData = notificationMessage.CustomData as NotificationMessage.MessageCustomData;
                            var deviceType = customData?.DeviceType ?? string.Empty;
                            await this.SendPushNotification(tokens, deviceType, notificationMessage, cancellationToken);
                        }

                        break;

                    case NotificationType.Email:
                        if (this.notificationSettings.EnableEmail)
                        {
                            var recipients = await this.GetMailRecipients(employeeIds, cancellationToken);
                            var customData = notificationMessage.CustomData as NotificationMessage.MessageCustomData;
                            var sender = customData?.Sender ?? string.Empty;
                            await this.SendEmailNotification(recipients, sender, notificationMessage,
                                cancellationToken);
                        }

                        break;
                }
            }
        }

        private async Task SendPushNotification(
            IDictionary<EmployeeId,
                IReadOnlyCollection<DeviceRegistryEntry>> deviceTokens,
            string deviceType,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken)
        {
            var notificationContent = new PushNotificationContent
            {
                Title = notificationMessage.Subject,
                Body = notificationMessage.ShortText,
                CustomData = new
                {
                    deviceType,
                    Type = notificationMessage
                        .NotificationTemplate // TODO: calculate correct value or remove this property
                }
            };

            foreach (var deviceToken in deviceTokens.Values)
            {
                await this.pushNotifications.SendPushNotification(deviceToken.ToArray(), notificationContent,
                    cancellationToken);
            }
        }

        private async Task SendEmailNotification(
            IReadOnlyCollection<string> recipients,
            string sender,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken)
        {
            var notificationContent = new EmailNotificationContent
            {
                Sender = sender,
                Subject = notificationMessage.Subject,
                Body = notificationMessage.LongText
            };

            await this.emailNotifications.SendEmailNotification(recipients.ToArray(), notificationContent,
                cancellationToken);
        }

        private async Task<(EmployeeId, UserPreferences)> GetUserPreferences(
            EmployeeId id, CancellationToken cancellationToken)
        {
            var preferences = await this.userPreferences.ForEmployee(id).Get(cancellationToken);
            return (id, preferences);
        }

        private bool IsPushNotification(
            IDictionary<EmployeeId, UserPreferences> userPreferencesDictionary, EmployeeId employeeId)
        {
            if (userPreferencesDictionary.TryGetValue(employeeId, out var pref))
            {
                return pref.PushNotifications;
            }

            return false;
        }

        private bool IsEmailNotification(
            IDictionary<EmployeeId, UserPreferences> userPreferencesDictionary, EmployeeId employeeId)
        {
            if (userPreferencesDictionary.TryGetValue(employeeId, out var pref))
            {
                return pref.EmailNotifications;
            }

            return false;
        }

        private async Task<IDictionary<EmployeeId, IReadOnlyCollection<DeviceRegistryEntry>>> GetDeviceTokens(
            IReadOnlyCollection<EmployeeId> employeeIds, CancellationToken cancellationToken)
        {
            var employeePreferences = (await Task.WhenAll(employeeIds
                    .Distinct()
                    .Select(x => this.GetUserPreferences(x, cancellationToken))))
                .ToDictionary(x => x.Item1, x => x.Item2);
            var activeEmployees = employeeIds.Where(x => this.IsPushNotification(employeePreferences, x));
            var tokens = await this.deviceRegistry.GetDeviceRegistryByEmployeeList(activeEmployees, cancellationToken);
            return tokens.Keys.SelectMany(k => tokens[k].Select(x =>
                    new
                    {
                        Key = k,
                        Val = x
                    }))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key,
                    x => (IReadOnlyCollection<DeviceRegistryEntry>)x.Select(a => a.Val).ToList().AsReadOnly());
        }

        private async Task<IReadOnlyCollection<string>> GetMailRecipients(
            IReadOnlyCollection<EmployeeId> employeeIds, CancellationToken cancellationToken)
        {
            var employeePreferences = (await Task.WhenAll(employeeIds
                    .Distinct()
                    .Select(x => this.GetUserPreferences(x, cancellationToken))))
                .ToDictionary(x => x.Item1, x => x.Item2);
            return (await Task.WhenAll(employeeIds
                    .Where(x => this.IsEmailNotification(employeePreferences, x))
                    .Select(x => this.employees.FindEmployeeAsync(x, cancellationToken))))
                .Where(x => x != null)
                .Select(x => x.Email)
                .ToList();
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
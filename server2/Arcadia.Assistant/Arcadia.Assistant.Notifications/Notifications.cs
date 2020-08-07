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

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Models;

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
        private readonly NotificationSettings notificationSettings;
        private readonly IPushNotifications pushNotifications;
        private readonly IUsersPreferencesStorage userPreferences;

        public Notifications(
            StatelessServiceContext context,
            NotificationSettings notificationSettings,
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
                this.logger.LogWarning("Notification declined: employees list is empty.");
                return;
            }

            IDictionary<EmployeeId, UserPreferences>? userPreferences = null;
            var notificationProviders =
                this.notificationProvidersMap.TryGetValue(notificationMessage.NotificationTemplate, out var map)
                    ? map
                    : this.AllNotificationTypes;
            foreach (var providerType in notificationProviders)
            {
                if (userPreferences == null)
                {
                    userPreferences = await this.GetUserPreferences(employeeIds, cancellationToken);
                }

                switch (providerType)
                {
                    case NotificationType.Push:
                        if (this.notificationSettings.EnablePush)
                        {
                            var tokens = await this.GetDeviceTokens(employeeIds, userPreferences, cancellationToken);
                            await this.SendPushNotification(tokens, notificationMessage, cancellationToken);
                        }
                        else
                        {
                            this.logger.LogDebug("Push notifications disabled");
                        }

                        break;

                    case NotificationType.Email:
                        if (this.notificationSettings.EnableEmail)
                        {
                            var employeeEmailAddresses =
                                await this.GetMailRecipients(employeeIds, userPreferences, cancellationToken);
                            await this.SendEmailNotification(employeeEmailAddresses, notificationMessage,
                                cancellationToken);
                        }
                        else
                        {
                            this.logger.LogDebug("Email notifications disabled");
                        }

                        break;
                }
            }
        }

        private async Task SendPushNotification(
            IDictionary<EmployeeId,
                IReadOnlyCollection<DeviceRegistryEntry>> deviceRegistrations,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken)
        {
            var notificationContent = new PushNotificationContent
            {
                Title = string.IsNullOrWhiteSpace(notificationMessage.Title)
                    ? notificationMessage.Subject
                    : notificationMessage.Title,
                Body = notificationMessage.ShortText,
                CustomData = new
                {
                    Type = notificationMessage
                        .NotificationTemplate // TODO: calculate correct value or remove this property
                }
            };

            try
            {
                var deviceTokens = deviceRegistrations.Values.SelectMany(x => x).Distinct();
                await this.pushNotifications.SendPushNotification(deviceTokens.ToArray(), notificationContent,
                    cancellationToken);

                this.logger.LogDebug("Push notifications has been sent.");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Sent push notifications error.");
            }
        }

        private async Task SendEmailNotification(
            IReadOnlyCollection<string> emailAddresses,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken)
        {
            var notificationContent = new EmailNotificationContent
            {
                Subject = string.IsNullOrWhiteSpace(notificationMessage.Subject)
                    ? notificationMessage.Title
                    : notificationMessage.Subject,
                Body = notificationMessage.LongText
            };

            try
            {
                await this.emailNotifications.SendEmailNotification(emailAddresses.ToArray(), notificationContent,
                    cancellationToken);

                this.logger.LogDebug("Email notifications has been sent.");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Send email notifications error.");
            }
        }

        private async Task<IDictionary<EmployeeId, UserPreferences>> GetUserPreferences(
            EmployeeId[] ids, CancellationToken cancellationToken)
        {
            var result = new Dictionary<EmployeeId, UserPreferences>();
            foreach (var id in ids.Distinct())
            {
                try
                {
                    var preferences = await this.userPreferences.ForEmployee(id).Get(cancellationToken);
                    result.Add(id, preferences);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Load preferences for user id={EmployeeId} error.", id);
                }
            }

            return result;
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
            IReadOnlyCollection<EmployeeId> employeeIds,
            IDictionary<EmployeeId, UserPreferences> employeePreferences,
            CancellationToken cancellationToken)
        {
            var activeEmployees = employeeIds.Where(x => this.IsPushNotification(employeePreferences, x));
            var tokens =
                await this.deviceRegistry.GetDeviceRegistryByEmployeeList(activeEmployees.ToArray(), cancellationToken);
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
            IReadOnlyCollection<EmployeeId> employeeIds,
            IDictionary<EmployeeId, UserPreferences> employeePreferences,
            CancellationToken cancellationToken)
        {
            return (await Task.WhenAll(employeeIds
                    .Where(x => this.IsEmailNotification(employeePreferences, x))
                    .Select(x => this.FindEmployeeAsync(x, cancellationToken))))
                .Where(x => x != null)
                .Select(x => x.Email)
                .ToList();
        }

        private async Task<EmployeeMetadata?> FindEmployeeAsync(
            EmployeeId employeeId, CancellationToken cancellationToken)
        {
            try
            {
                return await this.employees.FindEmployeeAsync(employeeId, cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Find employee with id={EmployeeId} error.", employeeId);
            }

            return null;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}
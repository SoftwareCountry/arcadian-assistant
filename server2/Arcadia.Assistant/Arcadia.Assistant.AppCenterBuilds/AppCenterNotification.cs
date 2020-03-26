namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DeviceRegistry.Contracts;
    using DeviceRegistry.Contracts.Models;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;
    using Notifications.Contracts.Models;

    public class AppCenterNotification : IAppCenterNotification
    {
        private readonly IDeviceRegistry deviceRegistry;
        private readonly ILogger logger;
        private readonly INotifications notifications;

        public AppCenterNotification(
            INotifications notifications,
            IDeviceRegistry deviceRegistry,
            ILogger<AppCenterNotification> logger)
        {
            this.notifications = notifications;
            this.deviceRegistry = deviceRegistry;
            this.logger = logger;
        }

        public async Task SendNewBuildNotification(
            string buildVersion, string mobileType, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send app center build notification about new build version {BuildVersion} for '{MobileType}' platform.",
                buildVersion, mobileType);

            try
            {
                var employees = await this.GetEmployeesByDeviceType(mobileType, cancellationToken);
                await this.notifications.Send(employees.ToArray(),
                    new NotificationMessage
                    {
                        NotificationTemplate = "NewBuildVersion",
                        Subject = "App center notification",
                        ShortText = $"New mobile build version {buildVersion} available",
                        LongText = $"New mobile build version {buildVersion} available"
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send mobile {DeviceType} with new version {Version} notification error", mobileType,
                    buildVersion);
                throw;
            }
        }

        private async Task<IReadOnlyCollection<EmployeeId>> GetEmployeesByDeviceType(
            string mobileType, CancellationToken cancellationToken)
        {
            return (await this.deviceRegistry.GetDeviceRegistryByDeviceType(new DeviceType(mobileType),
                    cancellationToken))
                .Keys;
        }
    }
}
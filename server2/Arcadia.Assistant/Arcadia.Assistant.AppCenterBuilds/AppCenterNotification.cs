namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;
    using Notifications.Contracts.Models;

    public class AppCenterNotification : IAppCenterNotification
    {
        private readonly ILogger logger;
        private readonly INotifications notifications;

        public AppCenterNotification(INotifications notifications, ILogger<AppCenterNotification> logger)
        {
            this.notifications = notifications;
            this.logger = logger;
        }

        async Task IAppCenterNotification.Notify(
            string notificationTemplate, string buildVersion, string mobileType, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send app center build notification about build version {BuildVersion} for '{MobileType}' platform.",
                buildVersion, mobileType);

            try
            {
                // TODO: Add employee array request
                await this.notifications.Send(new EmployeeId[0],
                    new NotificationMessage
                    {
                        NotificationTemplate = notificationTemplate,
                        Subject = "App center notification",
                        ShortText = $"New mobile build version {buildVersion} available",
                        LongText = $"New mobile build version {buildVersion} available",
                        Parameters = new Dictionary<string, string>
                        {
                            { NotificationMessage.KnowParameterNames.DeviceType, mobileType }
                        }
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send mobile {DeviceType} with version {Version} '{Notification}' notification error", mobileType,
                    buildVersion, notificationTemplate);
                throw;
            }
        }
    }
}
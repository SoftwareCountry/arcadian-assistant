namespace Arcadia.Assistant.AppCenterBuilds
{
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

            // TODO: Add employee list request
            await this.notifications.Send(new List<EmployeeId>(),
                new NotificationMessage
                {
                    NotificationTemplate = notificationTemplate,
                    Subject = "App center notification",
                    ShortText = $"New mobile build version {buildVersion} available",
                    LongText = $"New mobile build version {buildVersion} available",
                    Parameters = new Dictionary<string, string>
                    {
                        { NotificationMessage.ParameterNames.DeviceType, mobileType }
                    }
                },
                cancellationToken);
        }
    }
}
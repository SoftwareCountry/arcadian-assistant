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
        private readonly string clientName;
        private readonly ILogger logger;
        private readonly INotifications notifications;

        public AppCenterNotification(string clientName, INotifications notifications, ILogger logger)
        {
            this.clientName = clientName;
            this.notifications = notifications;
            this.logger = logger;
        }

        async Task IAppCenterNotification.Notify(string buildVersion, string mobileType, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"Send app center build notification about build version {buildVersion} for '{mobileType}' platform.");
            await this.notifications.Send(new List<EmployeeId>(),
                new NotificationMessage
                {
                    ClientName = this.clientName,
                    Subject = "App center notification",
                    ShortText = $"New mobile build version {buildVersion} available",
                    LongText = $"New mobile build version {buildVersion} available"
                },
                cancellationToken);
        }
    }
}
namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;
    using Notifications.Contracts.Models;

    using Organization.Contracts;

    public class AppCenterNotification : IAppCenterNotification
    {
        private readonly IEmployees employees;
        private readonly ILogger logger;
        private readonly INotifications notifications;
        private readonly IOrganization organization;

        public AppCenterNotification(
            INotifications notifications,
            IEmployees employees,
            IOrganization organization,
            ILogger<AppCenterNotification> logger)
        {
            this.notifications = notifications;
            this.employees = employees;
            this.organization = organization;
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
                var employeeIds = await this.GetEmployees(cancellationToken);
                // TODO: Add employee array request
                await this.notifications.Send(employeeIds,
                    new NotificationMessage
                    {
                        NotificationTemplate = notificationTemplate,
                        Subject = "App center notification",
                        ShortText = $"New mobile build version {buildVersion} available",
                        LongText = $"New mobile build version {buildVersion} available",
                        CustomData = new NotificationMessage.MessageCustomData
                        {
                            Sender = "AppCenter",
                            DeviceType = mobileType
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

        private async Task<EmployeeId[]> GetEmployees(CancellationToken cancellationToken)
        {
            var departmentIds = await this.organization.GetDepartmentsAsync(cancellationToken);
            return (await this.employees.FindEmployeesAsync(
                    EmployeesQuery.Create().ForDepartments(departmentIds.Select(x => x.DepartmentId.ToString())),
                    cancellationToken))
                .Select(x => x.EmployeeId)
                .ToArray();
        }
    }
}
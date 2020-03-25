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

    using Organization.Contracts;

    public class AppCenterNotification : IAppCenterNotification
    {
        private readonly IDeviceRegistry deviceRegistry;
        private readonly IEmployees employees;
        private readonly ILogger logger;
        private readonly INotifications notifications;
        private readonly IOrganization organization;

        public AppCenterNotification(
            INotifications notifications,
            IDeviceRegistry deviceRegistry,
            IEmployees employees,
            IOrganization organization,
            ILogger<AppCenterNotification> logger)
        {
            this.notifications = notifications;
            this.deviceRegistry = deviceRegistry;
            this.employees = employees;
            this.organization = organization;
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
                var employeesByDeviceType = await this.GetEmployeesByDeviceType(mobileType, cancellationToken);
                var employeesArray = await this.GetEmployees(employeesByDeviceType, cancellationToken);
                await this.notifications.Send(employeesArray,
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

        private async Task<EmployeeId[]> GetEmployees(
            IReadOnlyCollection<EmployeeId> employeesByDeviceType, CancellationToken cancellationToken)
        {
            var departmentIds = await this.organization.GetDepartmentsAsync(cancellationToken);
            return (await this.employees.FindEmployeesAsync(
                    EmployeesQuery.Create().ForDepartments(departmentIds.Select(x => x.DepartmentId.ToString())),
                    cancellationToken))
                .Where(x => employeesByDeviceType.Any(e => e == x.EmployeeId))
                .Select(x => x.EmployeeId)
                .ToArray();
        }
    }
}
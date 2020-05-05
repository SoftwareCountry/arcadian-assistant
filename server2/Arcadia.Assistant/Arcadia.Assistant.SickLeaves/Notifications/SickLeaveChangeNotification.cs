using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.SickLeaves.Notifications
{
    using System.Threading;
    using System.Threading.Tasks;

    using Assistant.Notifications.Contracts;
    using Assistant.Notifications.Contracts.Models;

    using CSP.Model;

    using Employees.Contracts;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using NotificationTemplates;
    using NotificationTemplates.Configuration;

    using Organization.Contracts;

    public abstract class SickLeaveChangeNotification
    {
        private readonly ArcadiaCspContext database;
        private readonly INotifications notificationService;
        private readonly IEmployees employeeService;
        private readonly IOrganization organizationService;
        private readonly ILogger logger;

        protected SickLeaveChangeNotification(
            ArcadiaCspContext database,
            INotifications notificationService,
            IEmployees employeeService,
            IOrganization organizationService,
            ILogger<SickLeaveChangeNotification> logger
        )
        {
            this.database = database;
            this.notificationService = notificationService;
            this.employeeService = employeeService;
            this.organizationService = organizationService;
            this.logger = logger;
        }

        public async Task SendNotification(
            EventId eventId,
            EmployeeId ownerEmployeeId,
            string notificationTemplate,
            INotificationConfiguration notificationConfiguration,
            CancellationToken cancellationToken)
        {
            var (owner, manager) = await GetNotificationEmployeesData(ownerEmployeeId, cancellationToken);
            var sickLeave = await GetSickLeaveData(eventId, ownerEmployeeId, cancellationToken);
            var contextData = this.GetNotificationContext(owner, sickLeave);
            var context = new NotificationDataPresenter(notificationConfiguration, contextData);
            var recipients = new List<EmployeeId> { ownerEmployeeId };
            if (manager != null)
            {
                recipients.Add(manager.EmployeeId);
            }

            await SendSickLeaveStatusNotification(
                notificationTemplate,
                context,
                recipients.ToArray(),
                cancellationToken);
        }

        private async Task<SickLeave> GetSickLeaveData(EventId eventId, EmployeeId ownerEmployeeId, CancellationToken cancellationToken)
        {
            return await this.database
                .SickLeaves
                .FirstOrDefaultAsync(x => x.Id == eventId 
                        && x.EmployeeId == ownerEmployeeId.Value, 
                    cancellationToken);
        }

        private async Task<(EmployeeMetadata, EmployeeMetadata?)> GetNotificationEmployeesData(EmployeeId ownerEmployeeId, CancellationToken cancellationToken)
        {
            var owner = await this.employeeService.FindEmployeeAsync(ownerEmployeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            var department = owner.DepartmentId == null ? null : await this.organizationService.GetDepartmentAsync(owner.DepartmentId.Value, cancellationToken);
            var isEmployeeChief = department?.ChiefId == ownerEmployeeId;
            var isHeadDepartment = department?.IsHeadDepartment ?? false;
            var parentDepartment = department?.ParentDepartmentId == null
                ? null
                : await this.organizationService.GetDepartmentAsync(department.ParentDepartmentId.Value,
                    CancellationToken.None);

            var managerEmployeeId = isEmployeeChief ? parentDepartment?.ChiefId : department?.ChiefId;
            if ((isHeadDepartment && isEmployeeChief)
                || managerEmployeeId == null)
            {
                return (owner!, null);
            }

            var manager = await this.employeeService.FindEmployeeAsync(managerEmployeeId.GetValueOrDefault(), cancellationToken);
            return (owner!, manager);
        }

        private Dictionary<string, string> GetNotificationContext(EmployeeMetadata ownerEmployee, SickLeave entity)
        {
            return new Dictionary<string, string>
            {
                ["employee"] = ownerEmployee.Name,
                ["employeeId"] = ownerEmployee.EmployeeId.ToString(),
                ["startDate"] = entity.Start.ToString("dd/MM/yyyy"),
                ["endDate"] = entity.End.ToString("dd/MM/yyyy")
            };
        }

        private async Task SendSickLeaveStatusNotification(
            string notificationTemplate,
            NotificationDataPresenter notificationContext,
            EmployeeId[] employees,
            CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send sick leave '{NotificationTemplate}' notification for {EmployeesCount} employees.",
                notificationTemplate, employees.Length);

            try
            {
                await this.notificationService.Send(employees,
                    new NotificationMessage
                    {
                        NotificationTemplate = notificationTemplate,
                        Title = notificationContext.Title,
                        Subject = notificationContext.Subject,
                        ShortText = notificationContext.ShortBody,
                        LongText = notificationContext.LongBody
                    },
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send sick leave '{NotificationTemplate}' notification for {EmployeesCount} employees error: {Error}",
                    notificationTemplate, employees.Length, e.Message);
                throw;
            }
        }
    }
}

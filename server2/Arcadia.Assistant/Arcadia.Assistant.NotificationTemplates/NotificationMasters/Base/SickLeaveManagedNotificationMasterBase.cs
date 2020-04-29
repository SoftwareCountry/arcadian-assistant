using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters.Base
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.NotificationTemplates.Interfaces;
    using Arcadia.Assistant.Sharepoint.NotificationTemplates;

    using Interfaces.Base;

    using Microsoft.Extensions.Logging;

    using Organization.Contracts;


    public abstract class SickLeaveManagedNotificationMasterBase : INotificationMaster
    {
        protected enum SickLeaveChangeType
        {
            [Display(Name = "Created")]
            Created,
            [Display(Name = "Prolonged")]
            Prolonged,
            [Display(Name = "Cancelled")]
            Cancelled
        }

        private readonly IEmployees employeeService;
        private readonly IOrganization organizationService;
        private readonly INotificationConfiguration configuration;

        protected ILogger Logger { get; }

        public SickLeaveManagedNotificationMasterBase(
            IEmployees employeeService,
            IOrganization organizationService,
            INotificationConfiguration configuration,
            ILogger logger
        )
        {
            this.employeeService = employeeService;
            this.organizationService = organizationService;
            this.configuration = configuration;
            this.Logger = logger;
        }

        public string NotificationSubject => this.configuration.Subject;

        public string NotificationTitle => this.configuration.Title;

        public async Task<BodyBuilder> GetNotificationMessageBody(Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken)
        {
            var changeType = this.ChangeType;
            var (owner, manager) = await GetEmployeesMetadata(ownerId, cancellationToken);

            this.Logger.LogDebug("Sending a sick leave '{NotificationAction}' email notification to manager {ManagerId} for user {OwnerId}",
                changeType, manager?.EmployeeId, ownerId);


            var templateExpressionContext = new Dictionary<string, string>
            {
                ["employee"] = owner.Name,
                ["employeeId"] = ownerId.ToString(),
                ["startDate"] = evt.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = evt.EndDate.ToString("dd/MM/yyyy")
            };

            return new BodyBuilder(this.configuration, templateExpressionContext);
        }

        protected abstract SickLeaveChangeType ChangeType { get; }

        private async Task<(EmployeeMetadata, EmployeeMetadata?)> GetEmployeesMetadata(EmployeeId ownerId, CancellationToken cancellationToken)
        {
            var owner = await this.employeeService.FindEmployeeAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                this.Logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            var department = owner.DepartmentId == null ? null : await this.organizationService.GetDepartmentAsync(owner.DepartmentId.Value, cancellationToken);
            var isEmployeeChief = department?.ChiefId == ownerId;
            var isHeadDepartment = department?.IsHeadDepartment ?? false;
            var parentDepartment = department?.ParentDepartmentId == null
                ? null
                : await this.organizationService.GetDepartmentAsync(department.ParentDepartmentId.Value,
                    cancellationToken);

            var managerEmployeeId = isEmployeeChief ? parentDepartment?.ChiefId : department?.ChiefId;
            if ((isHeadDepartment && isEmployeeChief) 
                || managerEmployeeId == null)
            {
                return (owner!, null);
            }

            var manager = await this.employeeService.FindEmployeeAsync(managerEmployeeId.GetValueOrDefault(), cancellationToken);
            return (owner!, manager);
        }
    }
}

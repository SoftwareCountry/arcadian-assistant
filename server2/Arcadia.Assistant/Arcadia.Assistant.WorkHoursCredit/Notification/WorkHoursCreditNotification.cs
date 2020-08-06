using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.WorkHoursCredit.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.Notifications.Contracts.Models;
    using Arcadia.Assistant.NotificationTemplates;
    using Arcadia.Assistant.NotificationTemplates.Configuration;
    using Arcadia.Assistant.Organization.Contracts;
    using Arcadia.Assistant.WorkHoursCredit.Contracts;
    using Arcadia.Assistant.WorkHoursCredit.Model;
    using Autofac.Features.OwnedInstances;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;

    public class WorkHoursCreditNotification
    {
        private readonly ILogger logger;
        private readonly Func<Owned<WorkHoursCreditContext>> dbFactory;
        private readonly IEmployees employeeService;
        private readonly IOrganization organizationService;
        private readonly Dictionary<string, INotificationConfiguration> notificationConfigurations;
        private readonly INotifications notifications;

        public WorkHoursCreditNotification(
            Func<Owned<WorkHoursCreditContext>> dbFactory,
            INotifications notifications,
            IEmployees employeeService,
            IOrganization organizationService,
            IWorkHoursCreditCreateNotificationConfiguration workHoursCreditCreateNotificationConfiguration,
            IWorkHoursCreditApproveRequireNotificationConfiguration workHoursCreditApproveRequireNotification,
            IWorkHoursCreditApproveNotificationConfiguration workHoursCreditApproveNotificationConfiguration,
            IWorkHoursCreditCancelNotificationConfiguration workHoursCreditCancelNotificationConfiguration,
            ILogger<WorkHoursCreditNotification> logger)
        {
            this.notifications = notifications;
            this.dbFactory = dbFactory;
            this.employeeService = employeeService;
            this.organizationService = organizationService;
            this.notificationConfigurations = new Dictionary<string, INotificationConfiguration>
            {
                { WorkHoursCreditNotificationTemplate.WorkHoursCreditCreated, workHoursCreditCreateNotificationConfiguration },
                { WorkHoursCreditNotificationTemplate.WorkHoursCreditApproveRequire, workHoursCreditApproveRequireNotification },
                { WorkHoursCreditNotificationTemplate.WorkHoursCreditApproved, workHoursCreditApproveNotificationConfiguration },
                { WorkHoursCreditNotificationTemplate.WorkHoursCreditCancelled, workHoursCreditCancelNotificationConfiguration },
            };
            this.logger = logger;
        }

        public async Task SendCreateWorkHoursCreditNotification(
            EmployeeId employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send work hours credits new request {Type} notification (employee id={EmployeeId}.",
                changeType, employeeId);

            var employee = await GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (employee == null)
            {
                this.logger.LogError("Can't find employee for new request notification.");
                throw new ArgumentException("Employee missed in database.");
            }

            try
            {
                var context = GetNotificationContext(employee!, changeType, date, dayPart);
                await this.notifications.Send(new EmployeeId[] { employeeId },
                    CreateNotificationMessage(
                        WorkHoursCreditNotificationTemplate.WorkHoursCreditCreated,
                        new NotificationDataPresenter(this.notificationConfigurations[WorkHoursCreditNotificationTemplate.WorkHoursCreditCreated], context)),
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send Workhours Credit notification error");
                throw;
            }
        }

        public async Task SendApproveRequireWorkHoursCreditNotification(EmployeeId employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart, CancellationToken cancellationToken)
        {
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find employee for work hours credit notification.");
                throw new ArgumentException("Employee missed in database.");
            }

            var manager = await this.GetEmployeeManagerMetadataAsync(owner, cancellationToken);
            if (manager == null)
            {
                this.logger.LogError("Can't find employee manager for approve require notification.");
                throw new ArgumentException("Employee manager missed in database.");
            }

            try
            {
                var context = GetNotificationContext(manager, changeType, date, dayPart);
                await this.notifications.Send(new EmployeeId[] { manager.EmployeeId },
                    CreateNotificationMessage(
                        WorkHoursCreditNotificationTemplate.WorkHoursCreditApproveRequire,
                        new NotificationDataPresenter(this.notificationConfigurations[WorkHoursCreditNotificationTemplate.WorkHoursCreditApproveRequire], context)),
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send Workhours Credit notification error");
                throw;
            }
        }

        public async Task SendApproveWorkHoursCreditNotification(EmployeeId employeeId, Guid requestId, CancellationToken cancellationToken)
        {
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find employee for work hours credit notification.");
                throw new ArgumentException("Employee missed in database.");
            }

            await SendStatusChangedNotification(
                owner,
                new EmployeeId[] { employeeId },
                WorkHoursCreditNotificationTemplate.WorkHoursCreditApproved,
                "Approved",
                requestId,
                cancellationToken);
        }

        public async Task SendRejectWorkHoursCreditNotification(EmployeeId employeeId, Guid requestId, string? rejectReason, CancellationToken cancellationToken)
        {
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find employee for work hours credit notification.");
                throw new ArgumentException("Employee missed in database.");
            }

            await SendStatusChangedNotification(owner,
                new EmployeeId[] { employeeId },
                WorkHoursCreditNotificationTemplate.WorkHoursCreditApproved,
                "Rejected", 
                requestId,
                cancellationToken);
        }

        public async Task SendCancelWorkHoursCreditNotification(EmployeeId employeeId, EmployeeId cancellerId, Guid requestId, string? cancellationReason, CancellationToken cancellationToken)
        {
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find employee for work hours credit notification.");
                throw new ArgumentException("Employee missed in database.");
            }

            var recipients = new List<EmployeeId> { employeeId };
            var manager = await this.GetEmployeeManagerMetadataAsync(owner, cancellationToken);
            if (manager != null && manager.EmployeeId != cancellerId)
            {
                recipients.Add(manager.EmployeeId);
            }
                
            await SendStatusChangedNotification(owner,
                recipients.ToArray(),
                WorkHoursCreditNotificationTemplate.WorkHoursCreditCancelled,
                "Cancelled",
                requestId,
                cancellationToken);
        }

        private async Task SendStatusChangedNotification(EmployeeMetadata owner, EmployeeId[] recipients, string template, string status, Guid requestId, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
            var request = await ctx.Value.ChangeRequests.FirstAsync(x => x.ChangeRequestId == requestId, cancellationToken);
            var context = GetNotificationContext(owner, request);
            context.Add("status", status);
            var dataPresenter = new NotificationDataPresenter(this.notificationConfigurations[template], context);

            try
            {
                await this.notifications.Send(recipients,
                    CreateNotificationMessage(
                        template,
                        dataPresenter),
                    cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    "Send Workhours Credit notification error");
                throw;
            }
        }

        private NotificationMessage CreateNotificationMessage(string notificationTemplate, NotificationDataPresenter notificationContext)
        {
            return new NotificationMessage
            {
                NotificationTemplate = notificationTemplate,
                Subject = notificationContext.Subject,
                Title = notificationContext.Title,
                ShortText = notificationContext.ShortBody,
                LongText = notificationContext.LongBody
            };
        }

        private async Task<EmployeeMetadata?> GetEmployeeMetadataAsync(EmployeeId? employeeId, CancellationToken cancellationToken)
        {
            return employeeId == null ? null : await this.employeeService.FindEmployeeAsync(employeeId.Value, cancellationToken);
        }

        private async Task<EmployeeMetadata?> GetEmployeeManagerMetadataAsync(EmployeeMetadata owner, CancellationToken cancellationToken)
        {
            var department = owner.DepartmentId == null
                ? null
                : await this.organizationService.GetDepartmentAsync(owner.DepartmentId.Value, cancellationToken);
            var isEmployeeChief = department?.ChiefId == owner.EmployeeId;
            var isHeadDepartment = department?.IsHeadDepartment ?? false;
            var parentDepartment = department?.ParentDepartmentId == null
                ? null
                : await this.organizationService.GetDepartmentAsync(department.ParentDepartmentId.Value,
                    CancellationToken.None);

            var managerEmployeeId = isEmployeeChief ? parentDepartment?.ChiefId : department?.ChiefId;
            if (isHeadDepartment && isEmployeeChief
                || managerEmployeeId == null)
            {
                return null;
            }

            return await GetEmployeeMetadataAsync(managerEmployeeId.GetValueOrDefault(), cancellationToken);
        }

        private Dictionary<string, string> GetNotificationContext(EmployeeMetadata ownerEmployee, WorkHoursChangeType type, DateTime date, DayPart dayPart)
        {
            return new Dictionary<string, string>
            {
                ["employee"] = ownerEmployee.Name,
                ["employeeId"] = ownerEmployee.EmployeeId.ToString(),
                ["type"] = type.ToString(),
                ["startDate"] = date.ToString("dd/MM/yyyy"),
                ["dayPart"] = dayPart.ToString()
            };
        }

        private Dictionary<string, string> GetNotificationContext(EmployeeMetadata ownerEmployee, ChangeRequest request)
        {
            var context = GetNotificationContext(ownerEmployee, request.ChangeType, request.Date, request.DayPart);
            return context;
        }
    }
}
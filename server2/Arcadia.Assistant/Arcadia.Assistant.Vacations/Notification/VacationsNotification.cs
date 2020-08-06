using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Vacations.Notification
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
    using Arcadia.Assistant.Vacations.Contracts;
    using Autofac.Features.OwnedInstances;
    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;

    public class VacationsNotification
    {
        private readonly ILogger logger;
        private readonly Func<Owned<VacationsStorage>> storageFactory;
        private readonly IEmployees employeeService;
        private readonly IOrganization organizationService;
        private readonly Dictionary<string, INotificationConfiguration> notificationConfigurations;
        private readonly INotifications notifications;

        public VacationsNotification(
            Func<Owned<VacationsStorage>> storageFactory,
            INotifications notifications,
            IEmployees employeeService,
            IOrganization organizationService,
            IVacationsStatusChangeNotificationConfiguration vacationsCreateNotificationConfiguration,
            IVacationsApproveRequireNotificationConfiguration vacationsApproveRequireNotificationConfiguration,
            ILogger<VacationsNotification> logger)
        {
            this.notifications = notifications;
            this.storageFactory = storageFactory;
            this.employeeService = employeeService;
            this.organizationService = organizationService;
            this.notificationConfigurations = new Dictionary<string, INotificationConfiguration>
            {
                { VacationsNotificationTemplate.VacationsStatusChanged, vacationsCreateNotificationConfiguration },
                { VacationsNotificationTemplate.VacationsApproveRequire, vacationsApproveRequireNotificationConfiguration }
            };
            this.logger = logger;
        }

        public async Task SendVacationCreateNotification(EmployeeId employeeId, VacationDescription vacationDescription, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send new employee (id={EmployeeId}) vacation request notification for dates {StartDate}-{EndDate}.",
                employeeId, vacationDescription.StartDate, vacationDescription.EndDate);

            var status = "Created";
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            await this.SendStatusChangedNotification(
                owner!, 
                employeeId, 
                VacationsNotificationTemplate.VacationsStatusChanged, 
                status, 
                vacationDescription, 
                cancellationToken);
        }

        public async Task SendVacationApproveRequireNotification(EmployeeId employeeId, VacationDescription vacationDescription, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send employee (id={EmployeeId}) vacation manager approve request notification for dates {StartDate}-{EndDate}.",
                employeeId, vacationDescription.StartDate, vacationDescription.EndDate);

            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            var manager = await this.GetEmployeeManagerMetadataAsync(owner, cancellationToken);
            if (manager == null)
            {
                this.logger.LogError("Can't find manager employee.");
                throw new ArgumentException("Manager employee missed in database.");
            }

            await this.SendStatusChangedNotification(
                owner!, 
                manager.EmployeeId, 
                VacationsNotificationTemplate.VacationsApproveRequire, 
                string.Empty, 
                vacationDescription, 
                cancellationToken);
        }

        public async Task SendVacationApproveNotification(EmployeeId employeeId, bool approve, int eventId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send employee (id={EmployeeId}) vacation (id={EventId}) {Action} notification.",
                employeeId, eventId, approve ? "Approve" : "Reject");

            var vacation = await GetVacationDescriptionAsync(employeeId, eventId, cancellationToken);
            var status = approve ?  "Approved" : "Rejected";
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            await this.SendStatusChangedNotification(
                owner!,
                employeeId,
                VacationsNotificationTemplate.VacationsApproveRequire,
                status,
                vacation,
                cancellationToken);
        }

        public async Task SendVacationCancelNotification(EmployeeId employeeId, EmployeeId cancellerId, string reason, int eventId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug(
                "Send employee (id={EmployeeId}) vacation (id={EventId}) Cancel notification.",
                employeeId, eventId);

            var vacation = await GetVacationDescriptionAsync(employeeId, eventId, cancellationToken);
            var status = "Cancelled";
            var owner = await this.GetEmployeeMetadataAsync(employeeId, cancellationToken);
            if (owner == null)
            {
                this.logger.LogError("Can't find owner employee.");
                throw new ArgumentException("Owner employee missed in database.");
            }

            var recipients = new List<EmployeeId> { employeeId };
            var manager = await this.GetEmployeeManagerMetadataAsync(owner, cancellationToken);
            if (manager != null && manager.EmployeeId != cancellerId)
            {
                recipients.Add(manager.EmployeeId);
            }

            await this.SendStatusChangedNotification(
                owner!,
                recipients.ToArray(),
                VacationsNotificationTemplate.VacationsApproveRequire,
                status,
                vacation,
                cancellationToken);
        }

        private async Task SendStatusChangedNotification(EmployeeMetadata owner, EmployeeId recipient, string template, string status, VacationDescription vacation, CancellationToken cancellationToken)
        {
            await SendStatusChangedNotification(owner, new EmployeeId[] { recipient }, template, status, vacation, cancellationToken);
        }

        private async Task SendStatusChangedNotification(EmployeeMetadata owner, EmployeeId[] recipients, string template, string status, VacationDescription vacation, CancellationToken cancellationToken)
        {
            var context = GetNotificationContext(owner, status, vacation.StartDate, vacation.EndDate);
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

        private async Task<VacationDescription> GetVacationDescriptionAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using var storage = this.storageFactory();
            return await storage.Value.GetCalendarEvent(employeeId, eventId, cancellationToken);
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

        private Dictionary<string, string> GetNotificationContext(EmployeeMetadata ownerEmployee, string status, DateTime startDate, DateTime endDate)
        {
            return new Dictionary<string, string>
            {
                ["employee"] = ownerEmployee.Name,
                ["employeeId"] = ownerEmployee.EmployeeId.ToString(),
                ["startDate"] = startDate.ToString("dd/MM/yyyy"),
                ["endDate"] = endDate.ToString("dd/MM/yyyy"),
                ["status"] = status,
            };
        }
    }
}
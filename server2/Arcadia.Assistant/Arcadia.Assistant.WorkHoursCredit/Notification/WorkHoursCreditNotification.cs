using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.WorkHoursCredit.Notification
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.WorkHoursCredit.Contracts;
    using Arcadia.Assistant.WorkHoursCredit.Model;
    using Autofac.Features.OwnedInstances;
    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;

    public class WorkHoursCreditNotification
    {
        private readonly ILogger logger;
        private readonly Func<Owned<WorkHoursCreditContext>> dbFactory;
        private readonly IEmployees employees;
        private readonly IWorkHoursCreditCreateNotificationConfiguration workHoursCreditCreateNotificationConfiguration;
        private readonly IWorkHoursCreditApproveRequireNotificationConfiguration workHoursCreditApproveRequireNotification;
        private readonly IWorkHoursCreditApproveNotificationConfiguration workHoursCreditApproveNotificationConfiguration;
        private readonly IWorkHoursCreditCancelNotificationConfiguration workHoursCreditCancelNotificationConfiguration;
        private readonly INotifications notifications;

        public WorkHoursCreditNotification(
            Func<Owned<WorkHoursCreditContext>> dbFactory,
            INotifications notifications,
            IEmployees employees,
            IWorkHoursCreditCreateNotificationConfiguration workHoursCreditCreateNotificationConfiguration,
            IWorkHoursCreditApproveRequireNotificationConfiguration workHoursCreditApproveRequireNotification,
            IWorkHoursCreditApproveNotificationConfiguration workHoursCreditApproveNotificationConfiguration,
            IWorkHoursCreditCancelNotificationConfiguration workHoursCreditCancelNotificationConfiguration,
            ILogger<WorkHoursCreditNotification> logger)
        {
            this.notifications = notifications;
            this.dbFactory = dbFactory;
            this.employees = employees;
            this.workHoursCreditCreateNotificationConfiguration = workHoursCreditCreateNotificationConfiguration;
            this.workHoursCreditApproveRequireNotification = workHoursCreditApproveRequireNotification;
            this.workHoursCreditApproveNotificationConfiguration = workHoursCreditApproveNotificationConfiguration;
            this.workHoursCreditCancelNotificationConfiguration = workHoursCreditCancelNotificationConfiguration;
            this.logger = logger;
        }

        public async Task SendCreateWorkHoursCreditNotification(
            EmployeeId employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart, CancellationToken cancellationToken)
        {
            //this.logger.LogDebug(
            //    "Send app center build notification about new build version {BuildVersion} for '{MobileType}' platform.",
            //    buildVersion, mobileType);

            try
            {
                /*
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
                */
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
            using var ctx = this.dbFactory();
        }

        public async Task SendApproveWorkHoursCreditNotification(EmployeeId employeeId, EmployeeId approverId, Guid requestId, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
        }

        public async Task SendRejectWorkHoursCreditNotification(EmployeeId employeeId, EmployeeId approverId, Guid requestId, string? rejectReason, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
        }

        public async Task SendCancelWorkHoursCreditNotification(EmployeeId employeeId, EmployeeId approverId, Guid requestId, string? cancellationReason, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
        }
    }
}
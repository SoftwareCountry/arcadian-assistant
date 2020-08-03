using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Vacations.Notification
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.Vacations.Contracts;
    using Autofac.Features.OwnedInstances;
    using Microsoft.Extensions.Logging;

    using Notifications.Contracts;

    public class VacationsNotification
    {
        private readonly ILogger logger;
        private readonly Func<Owned<VacationsStorage>> storageFactory;
        private readonly IEmployees employees;
        private readonly IVacationsStatusChangeNotificationConfiguration vacationsCreateNotificationConfiguration;
        private readonly IVacationsApproveRequireNotificationConfiguration vacationsApproveRequireNotificationConfiguration;
        private readonly INotifications notifications;

        public VacationsNotification(
            Func<Owned<VacationsStorage>> storageFactory,
            INotifications notifications,
            IEmployees employees,
            IVacationsStatusChangeNotificationConfiguration vacationsCreateNotificationConfiguration,
            IVacationsApproveRequireNotificationConfiguration vacationsApproveRequireNotificationConfiguration,
            ILogger<VacationsNotification> logger)
        {
            this.notifications = notifications;
            this.storageFactory = storageFactory;
            this.employees = employees;
            this.vacationsCreateNotificationConfiguration = vacationsCreateNotificationConfiguration;
            this.vacationsApproveRequireNotificationConfiguration = vacationsApproveRequireNotificationConfiguration;
            this.logger = logger;
        }

        public async Task SendVacationCreateNotification(
            EmployeeId employeeId, CancellationToken cancellationToken)
        {
            //this.logger.LogDebug(
            //    "Send app center build notification about new build version {BuildVersion} for '{MobileType}' platform.",
            //    buildVersion, mobileType);

            try
            {
                var status = "Created";

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

        public async Task SendVacationApproveRequireNotification(EmployeeId employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            using var ctx = this.storageFactory();
        }

        public async Task SendVacationApproveNotification(EmployeeId employeeId, bool approve, CancellationToken cancellationToken)
        {
            using var ctx = this.storageFactory();
            var status = approve ?  "Approved" : "Rejected";
        }

        public async Task SendVacationCancelNotification(EmployeeId employeeId, string reason, CancellationToken cancellationToken)
        {
            using var ctx = this.storageFactory();
            var status = "Cancelled";
        }
    }
}
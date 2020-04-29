namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters
{
    using Employees.Contracts;

    using Base;

    using Interfaces;

    using Microsoft.Extensions.Logging;

    using Organization.Contracts;

    public class SickLeaveProlongedNotificationMaster : SickLeaveManagedNotificationMasterBase
    {
        public SickLeaveProlongedNotificationMaster(
            IEmployees employeeService,
            IOrganization organizationService,
            ISickLeaveCreatedConfiguration configuration,
            ILogger<SickLeaveProlongedNotificationMaster> logger
        ) : base(employeeService, organizationService, configuration, logger)
        {
        }

        protected override SickLeaveChangeType ChangeType => SickLeaveChangeType.Prolonged;
    }
}

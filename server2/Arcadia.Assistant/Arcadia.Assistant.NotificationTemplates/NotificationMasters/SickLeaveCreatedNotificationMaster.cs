namespace Arcadia.Assistant.Sharepoint.NotificationTemplates
{
    using Assistant.NotificationTemplates.Interfaces;

    using Employees.Contracts;

    using Assistant.NotificationTemplates.NotificationMasters;
    using Assistant.NotificationTemplates.NotificationMasters.Base;


    using Microsoft.Extensions.Logging;

    using Organization.Contracts;

    public class SickLeaveCreatedNotificationMaster : SickLeaveManagedNotificationMasterBase
    {
        public SickLeaveCreatedNotificationMaster(
            IEmployees employeeService,
            IOrganization organizationService,
            ISickLeaveCreatedConfiguration configuration,
            ILogger<SickLeaveCreatedNotificationMaster> logger
        ) : base(employeeService, organizationService, configuration, logger)
        {
        }

        protected override SickLeaveChangeType ChangeType => SickLeaveChangeType.Created;
    }
}

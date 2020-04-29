using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters
{
    using System.Threading;
    using Arcadia.Assistant.Employees.Contracts;
    using Arcadia.Assistant.NotificationTemplates.NotificationMasters.Base;
    using Assistant.NotificationTemplates.NotificationMasters;

    using Interfaces;

    using Microsoft.Extensions.Logging;

    using Organization.Contracts;

    using Sharepoint.NotificationTemplates;

    public class SickLeaveCancelledNotificationMaster : SickLeaveManagedNotificationMasterBase
    {
        public SickLeaveCancelledNotificationMaster(
            IEmployees employeeService,
            IOrganization organizationService,
            ISickLeaveCancelledConfiguration configuration,
            ILogger<SickLeaveCreatedNotificationMaster> logger
        ) : base(employeeService, organizationService, configuration, logger)
        {
        }

        protected override SickLeaveChangeType ChangeType => SickLeaveChangeType.Cancelled;
    }
}

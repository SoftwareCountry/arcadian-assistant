using Arcadia.Assistant.Employees.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters
{
    using Interfaces;

    using Microsoft.Extensions.Logging;

    public class SickLeaveAccountingNotificationMaster : INotificationMaster
    {
        private readonly ISickLeaveCreatedConfiguration configuration;
        private readonly ILogger logger;

        public SickLeaveAccountingNotificationMaster(
            ISickLeaveCreatedConfiguration configuration,
            ILogger<SickLeaveAccountingNotificationMaster> logger
        )
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string NotificationSubject => throw new NotImplementedException();

        public string NotificationTitle => throw new NotImplementedException();

        public Task<BodyBuilder> GetNotificationMessageBody(Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

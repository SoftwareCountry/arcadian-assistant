using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Employees.Contracts;

    using Assistant.NotificationTemplates;

    using Interfaces;

    using Microsoft.Extensions.Logging;

    public class EventUserGrantedApprovalNotificationMaster : INotificationMaster
    {
        private readonly IEmployees employeeService;
        private readonly IEventUserGrantedApprovalConfiguration configuration;
        private readonly ILogger logger;

        public EventUserGrantedApprovalNotificationMaster(
            IEmployees employeeService,
            IEventUserGrantedApprovalConfiguration configuration,
            ILogger<EventUserGrantedApprovalNotificationMaster> logger
        )
        {
            this.employeeService = employeeService;
            this.configuration = configuration;
            this.logger = logger;
        }

        public string NotificationSubject => throw new NotImplementedException();

        public string NotificationTitle => throw new NotImplementedException();

        public async Task<BodyBuilder> GetNotificationMessageBody(Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Sending email notification about user {ApproverId} granted approval for event {EventId} of {OwnerId}",
                approverId, evt.EventId, ownerId);

            // TODO: null?
            var approver = await this.employeeService.FindEmployeeAsync(approverId, cancellationToken);
            var datesStr = evt.StartDate == evt.EndDate
                ? evt.StartDate.ToString("dd/MM/yyyy")
                : $"{evt.StartDate:dd/MM/yyyy} - {evt.EndDate:dd/MM/yyyy}";

            var templateExpressionContext = new Dictionary<string, string>
            {
                ["eventType"] = evt.Type,
                ["dates"] = datesStr,
                ["approver"] = approver?.Name??String.Empty
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                evt.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            return new BodyBuilder(this.configuration, templateExpressionContext);
        }
    }
}

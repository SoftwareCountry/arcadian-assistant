using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia.Assistant.NotificationTemplates.NotificationMasters
{
    using System.Linq;
    using System.Threading;

    using Assistant.NotificationTemplates;

    using Employees.Contracts;

    using Interfaces;

    using Microsoft.Extensions.Logging;

    public class EventAssignedToApproverNotificationMaster : INotificationMaster
    {
        private readonly IEmployees employeeService;
        private readonly IEventAssignedToApproverConfiguration configuration;
        private readonly ILogger logger;

        public EventAssignedToApproverNotificationMaster(
            IEmployees employeeService,
            IEventAssignedToApproverConfiguration configuration,
            ILogger<EventAssignedToApproverNotificationMaster> logger
        )
        {
            this.employeeService = employeeService;
            this.configuration = configuration;
            this.logger = logger;
        }

        public string NotificationSubject => this.configuration.Subject;

        public string NotificationTitle => this.configuration.Title;

        public async Task<BodyBuilder> GetNotificationMessageBody(Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Sending email notification about event {EventId} of {OwnerId} assigned to {ApproverId}",
                evt.EventId, ownerId, approverId);

            // TODO: should we check on owner is null?
            var owner = await this.employeeService.FindEmployeeAsync(ownerId, cancellationToken);
            var datesStr = evt.StartDate == evt.EndDate
                ? evt.StartDate.ToString("dd/MM/yyyy")
                : $"{evt.StartDate:dd/MM/yyyy} - {evt.EndDate:dd/MM/yyyy}";

            var templateExpressionContext = new Dictionary<string, string>
            {
                ["eventType"] = evt.Type,
                ["dates"] = datesStr,
                ["employee"] = owner?.Name??String.Empty
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                evt.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            return new BodyBuilder(this.configuration, templateExpressionContext);
        }
    }
}

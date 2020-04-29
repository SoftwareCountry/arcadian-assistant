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

    public class EventStatusChangedNotificationMaster : INotificationMaster
    {
        private readonly IEventStatusChangedConfiguration configuration;
        private readonly ILogger logger;

        public EventStatusChangedNotificationMaster(
            IEventStatusChangedConfiguration configuration,
            ILogger<EventStatusChangedNotificationMaster> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string NotificationSubject => this.configuration.Subject;

        public string NotificationTitle => this.configuration.Title;

        public Task<BodyBuilder> GetNotificationMessageBody(Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Sending email notification about event {EventId} status changed to owner", evt.EventId);

            var datesStr = evt.StartDate == evt.EndDate
                ? evt.StartDate.ToString("dd/MM/yyyy")
                : $"{evt.StartDate:dd/MM/yyyy} - {evt.EndDate:dd/MM/yyyy}";

            var templateExpressionContext = new Dictionary<string, string>
            {
                ["eventType"] = evt.Type,
                ["dates"] = datesStr,
                ["eventStatus"] = evt.Status
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                evt.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            return Task.FromResult(new BodyBuilder(this.configuration, templateExpressionContext));
        }
    }
}

namespace Arcadia.Assistant.BirthdaysFeed
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP;
    using CSP.Model;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using UserFeeds.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class BirthdaysFeed : StatelessService, IBirthdaysFeed
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;
        private readonly ILogger logger;

        public BirthdaysFeed(StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> employeeQuery, ILogger logger)
            : base(context)
        {
            this.employeeQuery = employeeQuery;
            this.logger = logger;
        }

        public string ServiceType => Constants.ServiceType;

        public async Task<IEnumerable<FeedItem>> GetItems(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            using (var query = this.employeeQuery())
            {
                return (await query.Value.Get()
                        .Where(x => x.IsWorking && !x.IsDelete)
                        .Where(x => x.Birthday.HasValue && !x.FiringDate.HasValue)
                        .Where(x => x.Birthday.Value >= new DateTime(x.Birthday.Value.Year, startDate.Month, startDate.Day)
                            && x.Birthday.Value <= new DateTime(x.Birthday.Value.Year, endDate.Month, endDate.Day))
                        .ToListAsync(cancellationToken))
                    .Select(this.ConvertFeedMessage).ToArray();
            }
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        private FeedItem ConvertFeedMessage(Employee employee)
        {
            var employeeid = employee.Id.ToString();
            var date = employee.Birthday.Value;
            var pronoun = employee.Gender == "F" ? "her" : "his";
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var text = $"{title} celebrates {pronoun} birthday on {date.ToString("MMMM dd")}!";
            return new FeedItem
            {
                Id = $"employee-birthday-{employeeid}-at-{date}",
                Title = title,
                Text = text,
                Image = employee.Image,
                Date = date
            };
        }
    }
}
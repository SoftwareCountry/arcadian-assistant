using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.AnniversaryFeed
{
    using Arcadia.Assistant.UserFeeds.Contracts;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP;
    using CSP.Model;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AnniversaryFeed : StatelessService, IAnniversaryFeed
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;
        private readonly ILogger logger;

        public AnniversaryFeed(StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> employeeQuery, ILogger logger)
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
                        .Where(x => x.IsWorking) // fired employes also
                        .Where(x => x.HiringDate >= new DateTime(x.HiringDate.Year, startDate.Month, startDate.Day) && x.HiringDate <= new DateTime(x.HiringDate.Year, endDate.Month, endDate.Day))
                        .ToListAsync(cancellationToken))
                    .Select(ConvertFeedMessage).ToArray();
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        #region private methods

        private FeedItem ConvertFeedMessage(Employee employee)
        {
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var date = DateTime.UtcNow;
            var msg = $"Congratulations with Anniversary! {YearsServedAt(employee, date)} years served!";
            return new FeedItem()
            {
                Id = $"employee-anniversary-{employee.ToString()}-at-{date}",
                Text = title,
                Description = msg,
                Image = employee.Image,
                Date = date
            };
        }

        public int? YearsServedAt(Employee employee, DateTime date)
        {
            DateTime toDate;
            if (employee.FiringDate == null)
            {
                toDate = date;
            }
            else
            {
                toDate = date > employee.FiringDate ? employee.FiringDate.Value : date;
            }

            return CalculateYearsFromDate(employee.HiringDate, toDate);
        }

        private static int? CalculateYearsFromDate(DateTime? fromDate, DateTime? toDate = null)
        {
            if (fromDate == null)
            {
                return null;
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            var years = toDate.Value.Year - fromDate.Value.Year;

            if ((fromDate.Value.Month > toDate.Value.Month) || ((fromDate.Value.Month == toDate.Value.Month) && (fromDate.Value.Day > toDate.Value.Day)))
            {
                years = years - 1;
            }

            return years;
        }

        #endregion
    }
}

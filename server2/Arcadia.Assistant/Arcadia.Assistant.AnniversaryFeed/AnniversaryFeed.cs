namespace Arcadia.Assistant.AnniversaryFeed
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
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using UserFeeds.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AnniversaryFeed : StatelessService, IAnniversaryFeed
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;
        private readonly ILogger logger;

        public AnniversaryFeed(
            StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> employeeQuery,
            ILogger<AnniversaryFeed> logger)
            : base(context)
        {
            this.employeeQuery = employeeQuery;
            this.logger = logger;
        }

        public string ServiceType => Constants.ServiceType;

        public async Task<FeedItem[]> GetItems(
            DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            using var query = this.employeeQuery();
            return (await query.Value.Get()
                    .Where(x => x.IsWorking) // fired employes also
                    .Where(x => x.HiringDate >= new DateTime(x.HiringDate.Year, startDate.Month, startDate.Day)
                        && x.HiringDate <= new DateTime(x.HiringDate.Year, endDate.Month, endDate.Day))
                    .ToListAsync(cancellationToken))
                .Select(this.ConvertFeedMessage)
                .ToArray();
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private FeedItem ConvertFeedMessage(Employee employee)
        {
            var employeeId = employee.Id.ToString();
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var date = DateTime.UtcNow;
            var text = $"Congratulations with Anniversary! {this.YearsServedAt(employee, date)} years served!";
            return new FeedItem
            {
                Id = $"employee-anniversary-{employeeId}-at-{date}",
                Title = title,
                Text = text,
                //Image = employee.Image, TODO: image removed to Avatar service
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

            if (fromDate.Value.Month > toDate.Value.Month ||
                fromDate.Value.Month == toDate.Value.Month && fromDate.Value.Day > toDate.Value.Day)
            {
                years -= 1;
            }

            return years;
        }
    }
}
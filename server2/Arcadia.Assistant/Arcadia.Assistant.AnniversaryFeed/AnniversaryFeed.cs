namespace Arcadia.Assistant.AnniversaryFeed
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Avatars.Contracts;

    using Contracts;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using UserFeeds.Contracts.Models;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AnniversaryFeed : StatelessService, IAnniversaryFeed
    {
        private readonly IAvatars avatarsService;
        private readonly IEmployees employeeService;
        private readonly ILogger logger;

        public AnniversaryFeed(
            StatelessServiceContext context,
            IEmployees employeeService,
            IAvatars avatarsService,
            ILogger<AnniversaryFeed> logger)
            : base(context)
        {
            this.employeeService = employeeService;
            this.avatarsService = avatarsService;
            this.logger = logger;
        }

        public string ServiceType => Constants.ServiceType;

        public async Task<FeedItem[]> GetItems(
            DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var employeesList =
                await this.employeeService.FindEmployeesAsync(
                    EmployeesQuery.Create().WithHireDateRange(startDate, endDate),
                    cancellationToken);
            this.logger.LogDebug("Received {ItemsCount} employees.", employeesList.Length);

            var result = new FeedItem[employeesList.Length];
            for (var idx = 0; idx < employeesList.Length; idx++)
            {
                result[idx] = await this.ConvertFeedMessage(employeesList[idx], cancellationToken);
            }

            return result;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private async Task<FeedItem> ConvertFeedMessage(EmployeeMetadata employee, CancellationToken cancellationToken)
        {
            var employeeId = employee.EmployeeId.ToString();
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var date = DateTime.UtcNow;
            var text = $"Congratulations with Anniversary! {this.YearsServedAt(employee, date)} years served!";
            return new FeedItem
            {
                Id = $"employee-anniversary-{employeeId}-at-{date}",
                Title = title,
                Text = text,
                Image = (await this.avatarsService.Get(employee.EmployeeId).GetPhoto(cancellationToken))?.Bytes,
                Date = date
            };
        }

        public int? YearsServedAt(EmployeeMetadata employee, DateTime date)
        {
            DateTime toDate;
            if (employee.FireDate == null)
            {
                toDate = date;
            }
            else
            {
                toDate = date > employee.FireDate ? employee.FireDate.Value : date;
            }

            return CalculateYearsFromDate(employee.HireDate, toDate);
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
namespace Arcadia.Assistant.BirthdaysFeed
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
    public class BirthdaysFeed : StatelessService, IBirthdaysFeed
    {
        private readonly IAvatars avatarsService;
        private readonly IEmployees employeeService;
        private readonly ILogger logger;

        public BirthdaysFeed(
            StatelessServiceContext context,
            IEmployees employeeService,
            IAvatars avatarsService,
            ILogger<BirthdaysFeed> logger)
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
            var employeesList = await this.employeeService.FindEmployeesAsync(
                EmployeesQuery.Create().WithBirthdayRange(startDate, endDate), cancellationToken);
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

        /// <summary>
        ///     Linq selector method to convert Employee information to FeedItem
        /// </summary>
        /// <param name="employee">Employee metadata object</param>
        /// <remarks>Ensure the employee birthday is not null</remarks>
        private async Task<FeedItem> ConvertFeedMessage(EmployeeMetadata employee, CancellationToken cancellationToken)
        {
            var employeeId = employee.EmployeeId.ToString();
            var date = employee.BirthDate.GetValueOrDefault();
            var pronoun = employee.Sex == Sex.Female ? "her" : "his";
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var text = $"{title} celebrates {pronoun} birthday on {date:MMMM dd)}!";
            return new FeedItem
            {
                Id = $"employee-birthday-{employeeId}-at-{date}",
                Title = title,
                Text = text,
                Image = (await this.avatarsService.Get(employee.EmployeeId).GetPhoto(cancellationToken))?.Bytes,
                Date = date
            };
        }
    }
}
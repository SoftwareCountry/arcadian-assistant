using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.BirthdaysFeed
{
    using Arcadia.Assistant.UserFeeds.Contracts;
    using Contracts;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class BirthdaysFeed : StatelessService, IBirthdaysFeed
    {
        private readonly ILogger logger;

        public BirthdaysFeed(StatelessServiceContext context, ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        public string ServiceType => Constants.ServiceType;

        public Task<IEnumerable<FeedItem>> GetItems(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }
    }
}

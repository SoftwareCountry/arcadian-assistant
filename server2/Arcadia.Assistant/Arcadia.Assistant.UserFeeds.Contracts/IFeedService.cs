namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IFeedService : IService
    {
        string ServiceType { get; }

        Task<IEnumerable<FeedItem>> GetItems(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
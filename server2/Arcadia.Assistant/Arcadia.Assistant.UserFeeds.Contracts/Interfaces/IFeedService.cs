namespace Arcadia.Assistant.UserFeeds.Contracts.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Models;

    public interface IFeedService
    {
        string ServiceType { get; }

        Task<FeedItem[]> GetItems(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFeedService
    {
        string ServiceType { get; }

        Task<FeedItem[]> GetItems(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
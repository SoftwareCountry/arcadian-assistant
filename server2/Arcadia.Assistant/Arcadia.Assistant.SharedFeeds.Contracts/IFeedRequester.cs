namespace Arcadia.Assistant.SharedFeeds.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFeedRequester
    {
        Task<ICollection<FeedMessage>> GetFeedMessages(DateTime date, CancellationToken cancellation);
    }
}

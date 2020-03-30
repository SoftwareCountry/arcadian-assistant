namespace Arcadia.Assistant.UserFeeds.Contracts.Interfaces
{
    using Models;

    public interface IFeed
    {
        FeedId Id { get; }

        string Name { get; }

        bool Subscribed { get; }
    }
}
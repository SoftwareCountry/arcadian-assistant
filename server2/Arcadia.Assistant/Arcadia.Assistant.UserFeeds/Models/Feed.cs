namespace Arcadia.Assistant.UserFeeds.Models
{
    using Contracts.Interfaces;
    using Contracts.Models;

    public class Feed : IFeed
    {
        public FeedId Id { get; set; } = new FeedId(string.Empty);

        public string Name { get; set; } = string.Empty;

        public bool Subscribed { get; set; } = true;
    }
}
namespace Arcadia.Assistant.UserFeeds.Contracts.Models
{
    public class Feed
    {
        public FeedId Id { get; set; } = new FeedId(string.Empty);

        public string Name { get; set; } = string.Empty;

        public bool IsSubscribed { get; set; } = true;
    }
}
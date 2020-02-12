namespace Arcadia.Assistant.UserFeeds
{
    using Contracts;

    public class Feed : IFeed
    {
        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool Subscribed { get; set; } = true;
    }
}
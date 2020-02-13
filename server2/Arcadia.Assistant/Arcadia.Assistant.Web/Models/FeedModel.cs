namespace Arcadia.Assistant.Web.Models
{
    using UserFeeds.Contracts;

    public class FeedModel : IFeed
    {
        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool Subscribed { get; set; } = false;
    }
}
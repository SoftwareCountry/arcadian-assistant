namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;

    public class FeedItem
    {
        public string Id { get; set; } = string.Empty;

        public string FeedType { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public byte[]? Image { get; set; }

        public DateTime Date { get; set; }
    }
}
namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;
    public class FeedItem
    {
        public string Id { get; set; }
        public string FeedType { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public DateTime Date { get; set; }
    }
}
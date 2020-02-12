namespace Arcadia.Assistant.UserFeeds.Contracts
{
    public interface IFeed
    {
        string Type { get; }
        string Name { get; }
        bool Subscribed { get; set; }
    }
}
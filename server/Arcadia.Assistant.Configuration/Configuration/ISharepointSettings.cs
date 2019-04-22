namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ISharepointSettings
    {
        string ServerUrl { get; }

        string ClientId { get; }

        string ClientSecret { get; }

        string CalendarEventIdField { get; }
    }
}
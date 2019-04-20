namespace Arcadia.Assistant.Configuration.Configuration
{
    public class SharepointSettings : ISharepointSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CalendarEventIdMapping { get; set; }
    }
}
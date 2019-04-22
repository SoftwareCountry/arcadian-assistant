namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class SharepointSettings : ISharepointSettings
    {
        [Required]
        public string ServerUrl { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        public string CalendarEventIdField { get; set; }
    }
}
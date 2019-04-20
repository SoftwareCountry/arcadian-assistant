namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public interface ISharepointSettings
    {
        [Required]
        string ClientId { get; }

        [Required]
        string ClientSecret { get; }

        string CalendarEventIdMapping { get; }
    }
}
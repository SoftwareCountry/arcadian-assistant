namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class MessagingSettings
    {
        [Required]
        public SmtpSettings Smtp { get; set; }

        [Required]
        public PushSettings Push { get; set; }

        [Required]
        public CalendarEventsMessagingSettings CalendarEvents { get; set; }
    }
}

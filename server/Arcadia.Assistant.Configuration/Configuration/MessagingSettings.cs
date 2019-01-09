namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class MessagingSettings
    {
        [Required]
        public SmtpSettings Smtp { get; set; }

        [Required]
        public ImapSettings Imap { get; set; }

        [Required]
        public PushSettings Push { get; set; }

        [Required]
        public CalendarEventsMailSettings CalendarEventsMail { get; set; }

        [Required]
        public CalendarEventsPushSettings CalendarEventsPush { get; set; }
    }
}

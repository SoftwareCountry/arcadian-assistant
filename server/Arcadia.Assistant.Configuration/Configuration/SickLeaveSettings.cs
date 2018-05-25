namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class SickLeaveSettings : IEmailSettings

    {
        [Required]
        public string NotificationRecipient { get; set; }

        [Required]
        public string NotificationSender { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}

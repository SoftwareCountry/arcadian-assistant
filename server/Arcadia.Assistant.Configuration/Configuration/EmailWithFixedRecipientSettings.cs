namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class EmailWithFixedRecipientSettings : EmailSettings, IEmailWithFixedRecipientSettings
    {
        [Required]
        public string NotificationRecipient { get; set; }
    }
}
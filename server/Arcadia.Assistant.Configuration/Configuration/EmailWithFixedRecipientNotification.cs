namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class EmailWithFixedRecipientNotification : EmailNotification, IEmailWithFixedRecipientNotification
    {
        [Required]
        public string NotificationRecipient { get; set; }
    }
}
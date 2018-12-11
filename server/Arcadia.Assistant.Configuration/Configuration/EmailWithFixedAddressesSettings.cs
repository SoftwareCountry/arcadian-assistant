namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class EmailWithFixedAddressesSettings : EmailSettings
    {
        [Required]
        public string NotificationRecipient { get; set; }

        [Required]
        public string NotificationSender { get; set; }
    }
}
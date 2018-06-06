namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class AppSettings
    {
        [Required]
        public MessagingSettings Messaging { get; set; }

        [Required]
        public OrganizationSettings Organization { get; set; }
    }
}
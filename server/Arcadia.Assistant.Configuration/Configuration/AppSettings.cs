namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.Extensions.Configuration;

    public class AppSettings
    {
        [Required]
        public MessagingSettings Messaging { get; set; }

        [Required]
        public OrganizationSettings Organization { get; set; }

        public ApplicationInsightsSettings ApplicationInsights { get; set; }

        [Required]
        public string Akka { get; set; }
    }
}
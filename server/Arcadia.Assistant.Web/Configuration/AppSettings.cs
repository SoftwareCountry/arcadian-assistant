namespace Arcadia.Assistant.Web.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AppSettings : ITimeoutSettings
    {
        [Required]
        public int TimeoutSeconds { get; set; }

        public TimeSpan Timeout => TimeSpan.FromSeconds(this.TimeoutSeconds);

        [Required]
        public ServerSettings Server { get; set; }

        [Required]
        public SecuritySettings Security { get; set; }

        public string Akka { get; set; }
    }
}
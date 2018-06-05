namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class SmtpSettings : ISmtpSettings
    {
        [Required]
        public string User { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }
        [Required]
        public bool UseTls { get; set; }
    }
}

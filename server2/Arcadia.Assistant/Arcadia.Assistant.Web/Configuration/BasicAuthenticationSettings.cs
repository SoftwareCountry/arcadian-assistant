namespace Arcadia.Assistant.Web.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class BasicAuthenticationSettings : IBasicAuthenticationSettings
    {
        [Required]
        public string? Realm { get; set; }

        [Required]
        public string? Login { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
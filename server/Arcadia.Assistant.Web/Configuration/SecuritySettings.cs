namespace Arcadia.Assistant.Web.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class SecuritySettings : ISecuritySettings
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string AuthorizationUrl { get; set; }

        [Required]
        public string TokenUrl { get; set; }

        [Required]
        public string SwaggerRedirectUri { get; set; }

        [Required]
        public string OpenIdConfigurationUrl { get; set; }
    }
}
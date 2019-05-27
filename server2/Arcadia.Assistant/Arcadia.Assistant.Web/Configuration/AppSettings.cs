namespace Arcadia.Assistant.Web.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class AppSettings
    {
        [Required]
        public SecuritySettings Security { get; set; }
    }
}
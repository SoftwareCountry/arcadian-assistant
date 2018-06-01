namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class OrganizationSettings
    {
        [Required]
        public RefreshInformation RefreshInformation { get; set; }
    }
}
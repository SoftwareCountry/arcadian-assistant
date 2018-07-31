namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class ConnectionStringsSettings
    {
        [Required]
        public string ArcadiaCSP { get; set; }
    }
}
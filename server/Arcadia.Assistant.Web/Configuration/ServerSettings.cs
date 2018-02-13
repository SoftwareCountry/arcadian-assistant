namespace Arcadia.Assistant.Web.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class ServerSettings
    {
        [Required]
        public string ActorSystemName { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string Host { get; set; }
    }
}
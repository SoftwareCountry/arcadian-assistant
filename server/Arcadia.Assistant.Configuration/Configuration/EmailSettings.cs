namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class EmailSettings
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
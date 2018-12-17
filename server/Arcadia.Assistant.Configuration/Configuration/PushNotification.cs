namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class PushNotification : IPushNotification
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
namespace Arcadia.Assistant.PushNotification.Models
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    public class PushNotification : IPushNotification
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
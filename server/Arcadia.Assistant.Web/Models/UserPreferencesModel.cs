namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserPreferencesModel
    {
        [Required]
        public bool EmailNotifications { get; set; }

        [Required]
        public bool PushNotifications { get; set; }
    }
}
namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Assistant.UserPreferences;

    public class UserPreferencesModel
    {
        [Required]
        public bool EmailNotifications { get; set; }

        [Required]
        public bool PushNotifications { get; set; }

        [Required]
        public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; set; }
    }
}
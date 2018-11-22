namespace Arcadia.Assistant.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Assistant.UserPreferences;

    public class UserPreferencesModel
    {
        public static UserPreferencesModel Default = new UserPreferencesModel
        {
            EmailNotifications = true,
            PushNotifications = true,
            DependentDepartmentsPendingActions = DependentDepartmentsPendingActions.HeadsOnly
        };

        [Required]
        public bool EmailNotifications { get; set; }

        [Required]
        public bool PushNotifications { get; set; }

        [Required]
        public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; set; }
    }
}
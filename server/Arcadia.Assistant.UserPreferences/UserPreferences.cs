namespace Arcadia.Assistant.UserPreferences
{
    public class UserPreferences
    {
        public bool EmailNotifications { get; set; }

        public bool PushNotifications { get; set; }

        public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; set; }
    }
}
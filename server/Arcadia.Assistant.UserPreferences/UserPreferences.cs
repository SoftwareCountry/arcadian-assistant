namespace Arcadia.Assistant.UserPreferences
{
    public class UserPreferences
    {
        public static UserPreferences Default = new UserPreferences
        {
            EmailNotifications = true,
            PushNotifications = true,
            DependentDepartmentsPendingActions = DependentDepartmentsPendingActions.HeadsOnly
        };

        public bool EmailNotifications { get; set; }

        public bool PushNotifications { get; set; }

        public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; set; }
    }
}
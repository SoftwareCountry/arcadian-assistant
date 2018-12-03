namespace Arcadia.Assistant.UserPreferences
{
    public class UserPreferences
    {
        public static UserPreferences Default = new UserPreferences
        {
            EmailNotifications = true,
            PushNotifications = true,
        };

        public bool EmailNotifications { get; set; }

        public bool PushNotifications { get; set; }
    }
}
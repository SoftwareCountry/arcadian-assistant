namespace Arcadia.Assistant.UserPreferences
{
    public class UserChangesPreferencesEvent
    {
        public UserChangesPreferencesEvent(string userId, UserPreferences userPreferences)
        {
            this.UserId = userId;
            this.UserPreferences = userPreferences;
        }

        public string UserId { get; }

        public UserPreferences UserPreferences { get; }
    }
}
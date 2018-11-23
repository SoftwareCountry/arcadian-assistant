namespace Arcadia.Assistant.UserPreferences
{
    public class SaveUserPreferencesMessage
    {
        public SaveUserPreferencesMessage(string userId, UserPreferences userPreferences)
        {
            this.UserId = userId;
            this.UserPreferences = userPreferences;
        }

        public string UserId { get; }

        public UserPreferences UserPreferences { get; }

        public class Response
        {
        }
    }
}
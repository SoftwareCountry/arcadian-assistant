namespace Arcadia.Assistant.UserPreferences
{
    public class GetUserPreferencesMessage
    {
        public GetUserPreferencesMessage(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }

        public class Response
        {
            public Response(UserPreferences userPreferences)
            {
                UserPreferences = userPreferences;
            }

            public UserPreferences UserPreferences { get; }
        }
    }
}
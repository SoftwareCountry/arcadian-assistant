namespace Arcadia.Assistant.UserPreferences
{
    using System.Runtime.Serialization;

    [DataContract]
    public class UserChangesPreferencesEvent
    {
        public UserChangesPreferencesEvent(string userId, UserPreferences userPreferences)
        {
            this.UserId = userId;
            this.UserPreferences = userPreferences;
        }

        [DataMember]
        public string UserId { get; }

        [DataMember]
        public UserPreferences UserPreferences { get; }
    }
}
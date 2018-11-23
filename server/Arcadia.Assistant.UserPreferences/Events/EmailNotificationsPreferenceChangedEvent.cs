namespace Arcadia.Assistant.UserPreferences.Events
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailNotificationsPreferenceChangedEvent
    {
        public EmailNotificationsPreferenceChangedEvent(string userId, bool emailNotifications)
        {
            this.UserId = userId;
            this.EmailNotifications = emailNotifications;
        }

        [DataMember]
        public string UserId { get; }

        [DataMember]
        public bool EmailNotifications { get; }
    }
}
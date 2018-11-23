namespace Arcadia.Assistant.UserPreferences.Events
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationsPreferenceChangedEvent
    {
        public PushNotificationsPreferenceChangedEvent(string userId, bool pushNotifications)
        {
            this.UserId = userId;
            this.PushNotifications = pushNotifications;
        }

        [DataMember]
        public string UserId { get; }

        [DataMember]
        public bool PushNotifications { get; }
    }
}
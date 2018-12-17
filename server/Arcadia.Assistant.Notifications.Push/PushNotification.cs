namespace Arcadia.Assistant.Notifications.Push
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotification
    {
        [DataMember(Name = "notification_content")]
        public PushNotificationContent Content { get; set; }

        [DataMember(Name = "notification_target")]
        public PushNotificationTarget Target { get; set; }
    }
}
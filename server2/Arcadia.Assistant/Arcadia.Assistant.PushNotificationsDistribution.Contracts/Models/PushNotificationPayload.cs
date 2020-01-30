namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationPayload
    {
        [DataMember(Name = "notification_content")]
        public PushNotificationContent Content { get; set; } = new PushNotificationContent();

        [DataMember(Name = "notification_target")]
        public PushNotificationTarget Target { get; set; } = new PushNotificationTarget();
    }
}
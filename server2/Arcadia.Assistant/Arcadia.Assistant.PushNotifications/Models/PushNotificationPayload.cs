namespace Arcadia.Assistant.PushNotifications.Models
{
    using System.Runtime.Serialization;

    using Contracts.Models;

    [DataContract]
    public class PushNotificationPayload
    {
        [DataMember(Name = "notification_content")]
        public PushNotificationContent Content { get; set; } = new PushNotificationContent();

        [DataMember(Name = "notification_target")]
        public PushNotificationTarget Target { get; set; } = new PushNotificationTarget();
    }
}
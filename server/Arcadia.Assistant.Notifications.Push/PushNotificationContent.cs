namespace Arcadia.Assistant.Notifications.Push
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationContent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember(Name = "custon_data")]
        public object CustomData { get; set; }
    }
}
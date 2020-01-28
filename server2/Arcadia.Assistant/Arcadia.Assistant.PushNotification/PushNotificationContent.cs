namespace Arcadia.Assistant.Notifications.Push
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationContent
    {
        [DataMember]
        public const string Name = "Arcadia Assistant API";

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember(Name = "custom_data")]
        public object CustomData { get; set; }

        public class ContentCustomData
        {
            public string Type { get; set; }
        }
    }
}
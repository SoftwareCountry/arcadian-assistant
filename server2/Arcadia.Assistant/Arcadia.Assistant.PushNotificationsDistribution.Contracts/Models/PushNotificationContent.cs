namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationContent
    {
        [DataMember] public const string Name = "Arcadia Assistant API";

        [DataMember]
        public string Title { get; set; } = string.Empty;

        [DataMember]
        public string Body { get; set; } = string.Empty;

        [DataMember(Name = "custom_data")]
        public object CustomData { get; set; } = string.Empty;

        public class ContentCustomData
        {
            public string Type { get; set; } = string.Empty;
        }
    }
}
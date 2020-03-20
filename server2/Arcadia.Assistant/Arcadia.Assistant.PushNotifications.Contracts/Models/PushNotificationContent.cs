namespace Arcadia.Assistant.PushNotifications.Contracts.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(IReadOnlyDictionary<string, string>))]
    public class PushNotificationContent
    {
        [DataMember] public const string Name = "Arcadia Assistant API";

        [DataMember]
        public string Title { get; set; } = string.Empty;

        [DataMember]
        public string Body { get; set; } = string.Empty;

        [IgnoreDataMember]
        public IReadOnlyDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        [DataMember(Name = "custom_data")]
        public object CustomData { get; set; } = string.Empty;

        public class ContentCustomData
        {
            public string Type { get; set; } = string.Empty;
        }
    }
}
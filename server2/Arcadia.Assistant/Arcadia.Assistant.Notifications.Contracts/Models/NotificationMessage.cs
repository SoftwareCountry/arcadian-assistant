namespace Arcadia.Assistant.Notifications.Contracts.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(IReadOnlyDictionary<string, string>))]
    [KnownType(typeof(Dictionary<string, string>))]
    public class NotificationMessage
    {
        [DataMember]
        public string NotificationTemplate { get; set; } = string.Empty;

        [DataMember]
        public string Subject { get; set; } = string.Empty;

        [DataMember]
        public string LongText { get; set; } = string.Empty;

        [DataMember]
        public string ShortText { get; set; } = string.Empty;

        [DataMember]
        public IReadOnlyDictionary<string, string> Parameters { get; set; } =
            new Dictionary<string, string>();

        public static class KnowParameterNames
        {
            public const string DeviceType = "DeviceType";
        }
    }
}
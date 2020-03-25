namespace Arcadia.Assistant.Notifications.Contracts.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(MessageCustomData))]
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
        public object CustomData { get; set; } = string.Empty;

        public class MessageCustomData
        {
            public string Sender { get; set; } = string.Empty;
        }
    }
}
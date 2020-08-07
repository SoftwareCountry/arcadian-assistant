namespace Arcadia.Assistant.Notifications.Contracts.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class NotificationMessage
    {
        [DataMember]
        public string NotificationTemplate { get; set; } = string.Empty;

        [DataMember]
        public string Title { get; set; } = string.Empty;

        [DataMember]
        public string Subject { get; set; } = string.Empty;

        [DataMember]
        public string LongText { get; set; } = string.Empty;

        [DataMember]
        public string ShortText { get; set; } = string.Empty;
    }
}
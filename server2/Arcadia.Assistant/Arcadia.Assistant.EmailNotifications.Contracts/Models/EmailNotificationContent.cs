namespace Arcadia.Assistant.EmailNotifications.Contracts.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailNotificationContent
    {
        [DataMember]
        public string Subject { get; set; } = string.Empty;

        [DataMember]
        public string Body { get; set; } = string.Empty;
    }
}
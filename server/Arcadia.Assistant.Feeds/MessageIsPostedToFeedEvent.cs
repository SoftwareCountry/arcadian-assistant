namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class MessageIsPostedToFeedEvent
    {
        [DataMember]
        public Guid MessageId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTimeOffset PostedDate { get; set; }
    }
}
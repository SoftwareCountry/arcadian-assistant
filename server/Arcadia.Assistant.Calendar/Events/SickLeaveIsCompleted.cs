namespace Arcadia.Assistant.Calendar.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SickLeaveIsCompleted
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public DateTime CompletionDate { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}
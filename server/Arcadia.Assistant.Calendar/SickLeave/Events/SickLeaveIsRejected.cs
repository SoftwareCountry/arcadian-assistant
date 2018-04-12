namespace Arcadia.Assistant.Calendar.SickLeave.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SickLeaveIsRejected
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
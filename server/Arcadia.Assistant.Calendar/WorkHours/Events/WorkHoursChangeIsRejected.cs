namespace Arcadia.Assistant.Calendar.WorkHours.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursChangeIsRejected
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}
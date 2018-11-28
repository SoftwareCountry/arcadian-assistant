namespace Arcadia.Assistant.Calendar.Vacations.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class VacationIsApprovedOnce
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}
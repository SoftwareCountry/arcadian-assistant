namespace Arcadia.Assistant.Calendar.WorkHours.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursDatesAreEdited
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}
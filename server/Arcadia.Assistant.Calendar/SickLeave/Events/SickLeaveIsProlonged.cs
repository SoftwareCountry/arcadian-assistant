namespace Arcadia.Assistant.Calendar.SickLeave.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SickLeaveIsProlonged
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }


    }
}
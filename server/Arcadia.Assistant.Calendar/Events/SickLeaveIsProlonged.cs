﻿namespace Arcadia.Assistant.Calendar.Events
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

        [DataMember]
        public string UserId { get; set; }
    }
}
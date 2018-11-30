namespace Arcadia.Assistant.Calendar.Vacations.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserGrantedVacationApproval
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string ApproverId { get; set; }
    }
}
namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SickLeaveDescription
    {
        [DataMember]
        public int SickLeaveId { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public SickLeaveStatus Status { get; set; }
    }
}
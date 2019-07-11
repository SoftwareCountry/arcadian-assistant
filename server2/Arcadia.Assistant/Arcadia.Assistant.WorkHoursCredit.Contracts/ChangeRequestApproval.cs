namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ChangeRequestApproval
    {
        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember]
        public string ApproverId { get; set; }
    }
}
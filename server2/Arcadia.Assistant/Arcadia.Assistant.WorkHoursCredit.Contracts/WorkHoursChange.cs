namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursChange
    {
        [DataMember]
        public Guid ChangeId { get; set; }

        [DataMember]
        public WorkHoursChangeType ChangeType { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public DayPart DayPart { get; set; }
    }
}
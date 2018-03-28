namespace Arcadia.Assistant.Organization.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmployeeChangedPosition
    {
        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string OldPosition { get; set; }

        [DataMember]
        public string NewPosition { get; set; }
    }
}
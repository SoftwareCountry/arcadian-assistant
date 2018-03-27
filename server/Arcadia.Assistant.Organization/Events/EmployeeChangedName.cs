namespace Arcadia.Assistant.Organization.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmployeeChangedName
    {
        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string OldName { get; set; }

        [DataMember]
        public string NewName { get; set; }
    }
}
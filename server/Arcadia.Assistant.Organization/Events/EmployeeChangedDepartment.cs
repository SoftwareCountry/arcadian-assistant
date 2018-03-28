namespace Arcadia.Assistant.Organization.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmployeeChangedDepartment
    {
        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string OldDepartmentName { get; set; }

        [DataMember]
        public string NewDepartmentName { get; set; }
    }
}
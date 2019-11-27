namespace Arcadia.Assistant.Organization
{
    using Contracts;
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class OrganizationDepartmentsReliableState
    {
        [DataMember]
        public int Version { get; set; } = 1;

        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember]
        public DepartmentMetadata[]? Data { get; set; }
    }
}
namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Diagnostics;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DepartmentMetadata
    {
        [DataMember]
        public DepartmentId DepartmentId { get; set; }

        [DataMember]
        public string Abbreviation { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DepartmentId? ParentDepartmentId { get; set; }

        [DataMember]
        public EmployeeId? ChiefId { get; set; }

        [DataMember]
        public int PeopleCount { get; set; }

        public bool IsHeadDepartment => this.ParentDepartmentId == null;

        public DepartmentMetadata(DepartmentId departmentId, string name, string abbreviation, DepartmentId? parentDepartmentId = null)
        {
            this.DepartmentId = departmentId;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.ParentDepartmentId = parentDepartmentId;
        }

        private string DebuggerDisplay => $"#{this.DepartmentId} - {this.Abbreviation}, Name: {this.Name}, Parent: {this.ParentDepartmentId}";
    }
}
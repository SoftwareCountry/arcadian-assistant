namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Diagnostics;
    using System.Runtime.Serialization;

    [DataContract]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DepartmentMetadata
    {
        [DataMember]
        public string DepartmentId { get; }

        [DataMember]
        public string Abbreviation { get; }

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string ParentDepartmentId { get; }

        [DataMember]
        public string ChiefId { get; set; }

        [DataMember]
        public int PeopleCount { get; set; }

        public bool IsHeadDepartment => this.ParentDepartmentId == null;

        public DepartmentMetadata(string departmentId, string name, string abbreviation, string parentDepartmentId = null)
        {
            this.DepartmentId = departmentId;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.ParentDepartmentId = parentDepartmentId;
        }

        private string DebuggerDisplay => $"#{this.DepartmentId} - {this.Abbreviation}, Name: {this.Name}, Parent: {this.ParentDepartmentId}";
    }
}
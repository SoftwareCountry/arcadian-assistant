namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Diagnostics;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DepartmentInfo
    {
        public string DepartmentId { get; }

        public string Abbreviation { get; }

        public string Name { get; }

        public string ParentDepartmentId { get; }

        public string ChiefId { get; set; }

        public bool IsHeadDepartment => this.ParentDepartmentId == null;

        public DepartmentInfo(string departmentId, string name, string abbreviation, string parentDepartmentId = null)
        {
            this.DepartmentId = departmentId;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.ParentDepartmentId = parentDepartmentId;
        }

        private string DebuggerDisplay => $"#{this.DepartmentId} - {this.Abbreviation}, Name: {this.Name}, Parent: {this.ParentDepartmentId}";
    }
}
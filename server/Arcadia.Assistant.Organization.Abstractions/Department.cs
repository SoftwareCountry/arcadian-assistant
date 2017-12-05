namespace Arcadia.Assistant.Organization.Abstractions
{
    public class Department
    {
        public string DepartmentId { get; }

        public string Name { get; }

        public string ParentDepartmentId { get; }

        public string ChiefId { get; set; }

        public Department(string departmentId, string name, string parentDepartmentId = null)
        {
            this.DepartmentId = departmentId;
            this.Name = name;
            this.ParentDepartmentId = parentDepartmentId;
        }
    }
}
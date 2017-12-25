namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;
    using System.Linq;

    public class DepartmentTree : Dictionary<string, DepartmentTree>
    {
        public DepartmentInfo Department { get; private set; }

        public DepartmentTree(DepartmentInfo department, DepartmentInfo[] allDepartments)
        {
            this.Department = department;

            foreach (var potentialChild in allDepartments)
            {
                if (potentialChild.ParentDepartmentId == department.DepartmentId)
                {
                    var potentialChildren = allDepartments.Where(x => x.DepartmentId != potentialChild.DepartmentId).ToArray();
                    this.Add(potentialChild.DepartmentId, new DepartmentTree(potentialChild, potentialChildren));
                }
            }
        }
    }
}
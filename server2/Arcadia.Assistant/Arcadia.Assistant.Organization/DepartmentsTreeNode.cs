namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    public class DepartmentsTreeNode
    {
        public int EmployeesCount { get; }

        public DepartmentMetadata DepartmentInfo { get; }

        public DepartmentsTreeNode(DepartmentMetadata departmentInfo, int employeesCount, IEnumerable<DepartmentsTreeNode> children)
        {
            this.DepartmentInfo = departmentInfo;
            this.EmployeesCount = employeesCount;
            this.Children = children.ToList();
        }

        public IReadOnlyCollection<DepartmentsTreeNode> Children { get; }

        public int CountAllEmployees()
        {
            return this.Children.Sum(x => x.CountAllEmployees()) + this.EmployeesCount;
        }

        public IEnumerable<DepartmentsTreeNode> AsEnumerable()
        {
            yield return this;

            foreach (var child in this.Children)
            {
                foreach (var d in child.AsEnumerable())
                {
                    yield return d;
                }
            }
        }
    }
}
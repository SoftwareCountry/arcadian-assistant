namespace Arcadia.Assistant.CSP
{
    using System.Collections.Generic;
    using System.Linq;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentsTreeNode
    {
        public int EmployeesCount { get; }

        public DepartmentInfo DepartmentInfo { get; }

        public DepartmentsTreeNode(DepartmentInfo departmentInfo, int employeesCount, IEnumerable<DepartmentsTreeNode> children)
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
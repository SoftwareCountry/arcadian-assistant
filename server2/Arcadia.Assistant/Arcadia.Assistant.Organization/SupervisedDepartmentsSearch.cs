namespace Arcadia.Assistant.Organization
{
    using Contracts;
    using Employees.Contracts;
    using System.Linq;

    public class SupervisedDepartmentsSearch
    {
        private readonly DepartmentMetadata[] allDepartments;
        private readonly DepartmentsTreeBuilder departmentsTreeBuilder;

        public SupervisedDepartmentsSearch(DepartmentMetadata[] allDepartments)
        {
            this.allDepartments = allDepartments;
            this.departmentsTreeBuilder = new DepartmentsTreeBuilder(allDepartments);
        }

        public DepartmentMetadata[] FindFor(EmployeeId employeeId)
        {
            var directlySupervisedDepartments = this.allDepartments.Where(x => x.ChiefId == employeeId);

            // ReSharper disable once RedundantEnumerableCastCall
            var trees = directlySupervisedDepartments.Select(x => this.departmentsTreeBuilder.Build(x.DepartmentId)).OfType<DepartmentsTreeNode>();

            var departments = trees.SelectMany(x => x.AsEnumerable().Select(tree => tree.DepartmentInfo)).Distinct().ToArray();
            return departments;
        }
    }
}
namespace Arcadia.Assistant.Organization
{
    using System.Linq;

    using Contracts;

    public class SupervisedDepartmentsSearch
    {
        private readonly DepartmentMetadata[] allDepartments;
        private readonly DepartmentsTreeBuilder departmentsTreeBuilder;

        public SupervisedDepartmentsSearch(DepartmentMetadata[] allDepartments)
        {
            this.allDepartments = allDepartments;
            this.departmentsTreeBuilder = new DepartmentsTreeBuilder(allDepartments);
        }

        public DepartmentMetadata[] FindFor(string employeeId)
        {
            var directlySupervisedDepartments = this.allDepartments.Where(x => x.ChiefId == employeeId);
            var trees = directlySupervisedDepartments.Select(x => this.departmentsTreeBuilder.Build(x.DepartmentId)).Where(x => x != null);
            var departments = trees.SelectMany(x => x.AsEnumerable().Select(tree => tree.DepartmentInfo)).Distinct().ToArray();
            return departments;
        }
    }
}
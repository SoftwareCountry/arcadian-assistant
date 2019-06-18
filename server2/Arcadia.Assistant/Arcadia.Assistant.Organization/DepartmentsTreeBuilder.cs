namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    public class DepartmentsTreeBuilder
    {
        private readonly IReadOnlyCollection<DepartmentMetadata> allDepartments;

        public DepartmentsTreeBuilder(IReadOnlyCollection<DepartmentMetadata> allDepartments)
        {
            this.allDepartments = allDepartments;
        }

        public DepartmentsTreeNode Build(string rootDepartmentId)
        {
            var root = this.allDepartments.FirstOrDefault(x => x.DepartmentId == rootDepartmentId);
            if (root == null)
            {
                return null;
            }

            var processedIds = new HashSet<string>
                { rootDepartmentId };
            var tree = new DepartmentsTreeNode(root, root.PeopleCount, this.CreateTree(root.DepartmentId, processedIds));
            return tree;
        }

        /// <param name="departmentId">Node for which descandants are requested</param>
        /// <param name="processedIds">a hashset with processed department ids to prevent stackoverflow</param>
        private List<DepartmentsTreeNode> CreateTree(
            string departmentId,
            HashSet<string> processedIds)
        {
            var childrenDepartments = this.allDepartments
                .Where(x => x.ParentDepartmentId == departmentId && !processedIds.Contains(x.DepartmentId))
                .ToList();

            processedIds.UnionWith(childrenDepartments.Select(x => x.DepartmentId));

            var children = new List<DepartmentsTreeNode>();
            foreach (var department in childrenDepartments)
            {
                var child = new DepartmentsTreeNode(department, department.PeopleCount, this.CreateTree(department.DepartmentId, processedIds));
                children.Add(child);
            }

            return children;
        }
    }
}
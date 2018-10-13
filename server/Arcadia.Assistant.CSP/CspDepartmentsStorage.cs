namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class CspDepartmentsStorage : DepartmentsStorage
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public CspDepartmentsStorage(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        private const int ArcadiaCompanyId = 154; //TODO: put in config

        private const string PriorityHeadDepartment = "GMG";

        private readonly Expression<Func<Department, IEnumerable<Employee>, DepartmentInfo>> mapDepartment =
            (x, chiefsGroup) =>
                new DepartmentInfo(
                    x.Id.ToString(),
                    x.Name,
                    x.Abbreviation,
                    (x.ParentDepartmentId == null) || (x.Id == x.ParentDepartmentId) ? null : x.ParentDepartmentId.ToString())
                { ChiefId = (x.ChiefId == null) || !chiefsGroup.Any() ? null : x.ChiefId.ToString() };

        private readonly Expression<Func<DepartmentInfo, IEnumerable<Employee>, DepartmentInfoWithPeopleCount>> countEmployees =
            (d, employees) =>
                new DepartmentInfoWithPeopleCount()
                {
                    Department = d,
                    PeopleCount = employees.Count()
                };

        protected override async Task<LoadAllDepartments.Response> GetAllDepartments()
        {
            using (var context = this.contextFactory())
            {
                var arcEmployees = new CspEmployeeQuery(context).Get();

                var allDepartments = await context
                    .Department
                    .Where(x => (x.IsDelete != true) && (x.CompanyId == ArcadiaCompanyId))
                    .GroupJoin(arcEmployees, d => d.ChiefId, e => e.Id, this.mapDepartment)
                    .GroupJoin(arcEmployees, d => d.DepartmentId, e => e.DepartmentId.ToString(), this.countEmployees)
                    .ToListAsync();

                var head = allDepartments.FirstOrDefault(x => x.Department.IsHeadDepartment && (x.Department.Abbreviation == PriorityHeadDepartment));

                if (head == null)
                {
                    return new LoadAllDepartments.Response(new DepartmentInfo[0]);
                }

                var tree = new DepartmentsTreeNode(head.Department, head.PeopleCount);
                tree.Children.AddRange(this.CreateTree(allDepartments, tree.DepartmentInfo.DepartmentId, new HashSet<string>() { head.Department.DepartmentId }));

                var departments = tree.AsEnumerable().Where(x => x.CountAllEmployees() != 0).Select(x => x.DepartmentInfo).ToList();

                return new LoadAllDepartments.Response(departments);
            }
        }

        private class DepartmentInfoWithPeopleCount
        {
            public DepartmentInfo Department { get; set; }

            public int PeopleCount { get; set; }
        }

        /// <param name="departmentId">Node for which descandants are requested</param>
        /// <param name="processedIds">a hashset with processed department ids to prevent stackoverflow</param>
        /// <param name="allDepartments">all possible departments</param>
        private List<DepartmentsTreeNode> CreateTree(IReadOnlyCollection<DepartmentInfoWithPeopleCount> allDepartments,
            string departmentId,
            HashSet<string> processedIds)
        {
            var children = allDepartments
                .Where(x => (x.Department.ParentDepartmentId == departmentId) && !processedIds.Contains(x.Department.DepartmentId))
                .Select(x => new DepartmentsTreeNode(x.Department, x.PeopleCount))
                .ToList();

            processedIds.UnionWith(children.Select(x => x.DepartmentInfo.DepartmentId));

            foreach (var child in children)
            {
                child.Children.AddRange(this.CreateTree(allDepartments, child.DepartmentInfo.DepartmentId, processedIds));
            }

            return children;
        }
    }
}
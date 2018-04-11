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

        private const int ArcadiaCompanyId = 154;

        private readonly Expression<Func<Department, DepartmentInfo>> mapDepartment =
            x =>
                new DepartmentInfo(
                    x.Id.ToString(),
                    x.Name,
                    x.Abbreviation,
                    (x.ParentDepartmentId == null) || (x.Id == x.ParentDepartmentId) ? null : x.ParentDepartmentId.ToString())
                    { ChiefId = x.ChiefId == null ? null : x.ChiefId.ToString() };

        private readonly Expression<Func<Department, bool>> isHeadDepartment = x => (x.CompanyId == 154) && (x.Id == x.ParentDepartmentId);

        protected override async Task<LoadHeadDepartment.Response> GetHeadDepartment()
        {
            using (var context = this.contextFactory())
            {
                var department = await context
                    .Department
                    .Where(this.isHeadDepartment)
                    .Select(this.mapDepartment)
                    .FirstOrDefaultAsync();

                return new LoadHeadDepartment.Response(department);
            }
        }

        protected override async Task<LoadChildDepartments.Response> GetChildDepartments(string departmentId)
        {
            var id = long.Parse(departmentId);

            using (var context = this.contextFactory())
            {
                var arcEmployees = context.Employee.FromSql(CspEmployeesInfoStorage.ArcadianEmployeeQuery);

                var departments = await context
                    .Department
                    .Where(x => (x.ParentDepartmentId == id) && (x.Id != id)) //TODO: deal with empty departments
                    .Where(x => (x.IsDelete != true))
                    .Join(arcEmployees, o => o.ChiefId, i => i.Id, (d, e) => d)
                    .Select(this.mapDepartment)
                    .ToListAsync();

                return new LoadChildDepartments.Response(departments);
            }
        }

        protected override async Task<LoadAllDepartments.Response> GetAllDepartments()
        {
            using (var context = this.contextFactory())
            {
                var arcEmployees = context.Employee.FromSql(CspEmployeesInfoStorage.ArcadianEmployeeQuery);

                var allDepartments = await context
                    .Department
                    .Where(x => (x.IsDelete != true) && (x.CompanyId == ArcadiaCompanyId))
                    .Join(arcEmployees, o => o.ChiefId, i => i.Id, (d, e) => d)
                    .Select(this.mapDepartment)
                    .ToListAsync();

                var head = allDepartments.FirstOrDefault(x => x.IsHeadDepartment);

                var filteredDepartments = new List<DepartmentInfo>();

                if (head != null)
                {
                    filteredDepartments.Add(head);
                    filteredDepartments.AddRange(GetDescendants(allDepartments, head, new HashSet<string>()));
                }

                return new LoadAllDepartments.Response(filteredDepartments);
            }
        }

        /// <param name="node">Node for which descandants are requested</param>
        /// <param name="processedIds">a hashset with processed department ids to prevent stackoverflow</param>
        /// <param name="allDepartments">all possible departments</param>
        private static List<DepartmentInfo> GetDescendants(
            IReadOnlyCollection<DepartmentInfo> allDepartments,
            DepartmentInfo node,
            HashSet<string> processedIds)
        {
            var children = allDepartments
                .Where(x => (x.ParentDepartmentId == node.DepartmentId) && !processedIds.Contains(x.DepartmentId))
                .ToList();

            processedIds.UnionWith(children.Select(x => x.DepartmentId));

            var descendants = new List<DepartmentInfo>(children);

            foreach (var child in children)
            {
                descendants.AddRange(GetDescendants(allDepartments, child, processedIds));
            }

            return descendants;
        }
    }
}
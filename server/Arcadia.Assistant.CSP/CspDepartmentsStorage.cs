namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    using Department = Arcadia.Assistant.Organization.Abstractions.Department;

    public class CspDepartmentsStorage : DepartmentsStorage
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public CspDepartmentsStorage(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        private Expression<Func<Model.Department, Department>> MapDepartment =
            x =>
                new Department(
                    x.Id.ToString(),
                    x.Name,
                    x.ParentDepartmentId == null ? null : x.ParentDepartmentId.ToString())
                    { ChiefId = x.ChiefId == null ? null : x.ChiefId.ToString() };

        protected override async Task<LoadHeadDepartment.Response> GetHeadDepartment()
        {
            using (var context = this.contextFactory())
            {
                var department = await context
                    .Department
                    .Where(x => (x.CompanyId == 154) && (x.Abbreviation == "CEO")) //TODO: fix hard code
                    .Select(this.MapDepartment)
                    .FirstOrDefaultAsync();

                return new LoadHeadDepartment.Response(department);
            }
        }

        protected override async Task<LoadChildDepartments.Response> GetChildDepartments(string departmentId)
        {
            var id = long.Parse(departmentId);

            using (var context = this.contextFactory())
            {
                var departments = await context
                    .Department
                    .Where(x => (x.ParentDepartmentId == id) && (x.Id != id)) //TODO: fix hard code
                    .Where(x => (x.IsDelete != true) && (x.Employee.Count > 0))
                    .Select(this.MapDepartment)
                    .ToListAsync();

                return new LoadChildDepartments.Response(departments);
            }
        }
    }
}
namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    using Department = Arcadia.Assistant.Organization.Abstractions.Department;

    public class CspDepartmentsQuery : DepartmentsQuery
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public CspDepartmentsQuery(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected override async Task<RequestAllDepartments.Response> GetAllDepartments()
        {
            using (var context = this.contextFactory())
            {
                var departments = await context
                    .Department
                    .Where(x => x.CompanyId == 154) //TODO: fix hard code
                    .Select(x => 
                        new Department(
                            x.Id.ToString(),
                            x.Name,
                            x.ParentDepartmentId.ToString())
                        { ChiefId = x.ChiefId.ToString() })
                    .ToArrayAsync();

                return new RequestAllDepartments.Response(departments);
            }
        }
    }
}
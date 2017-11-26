namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class AllEmployeesQueryImpl : AllEmployeesQuery
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public AllEmployeesQueryImpl(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected override async Task<RequestAllEmployeeIds.Response> GetAllEmployeeIds()
        {
            using (var context = this.contextFactory())
            {
                var ids = await context
                .Employee
                .Where(x => x.IsWorking && (x.Sid != null))
                .Select(x => x.Id.ToString())
                .ToArrayAsync();

                return new RequestAllEmployeeIds.Response(ids);
            }
        }
    }
}
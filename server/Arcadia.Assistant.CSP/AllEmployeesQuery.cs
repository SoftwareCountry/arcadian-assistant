namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class AllEmployeesQuery : IAllEmployeesQuery
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public AllEmployeesQuery(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<string[]> GetAllEmployeeIds()
        {
            using (var context = this.contextFactory())
            {
                try
                {
                    var a = await context.Employee.Where(x => x.IsWorking && (x.Sid != null)).Select(x => x.Sid.Value.ToString()).ToArrayAsync().ConfigureAwait(false);

                    return a;
                }
                catch (Exception e)
                {
                    return new string[]{ };
                }
            }
        }
    }
}
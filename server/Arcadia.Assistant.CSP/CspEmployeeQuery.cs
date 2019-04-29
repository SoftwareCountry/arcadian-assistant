namespace Arcadia.Assistant.CSP
{
    using System.Linq;

    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeeQuery
    {
        private readonly ArcadiaCspContext ctx;

        public CspEmployeeQuery(ArcadiaCspContext ctx)
        {
            this.ctx = ctx;
        }

        public IQueryable<Employee> Get()
        {
            return this.ctx.Employee
                .AsNoTracking()
                .Where(x => x.FiringDate == null && !x.IsDelete && x.CompanyId == 154 || x.Id == 145); // Don't really know what these constants are about, just copied from ArcadianEmployee view
        }
    }
}
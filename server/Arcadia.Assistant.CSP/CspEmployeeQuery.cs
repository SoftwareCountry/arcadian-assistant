namespace Arcadia.Assistant.CSP
{
    using System.Linq;

    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeeQuery
    {
        private readonly ArcadiaCspContext ctx;

        public const string ArcadianEmployeeQuery = "SELECT * FROM dbo.ArcadianEmployee WHERE IsDelete <> 1";

        public CspEmployeeQuery(ArcadiaCspContext ctx)
        {
            this.ctx = ctx;
        }

        public IQueryable<Employee> Get()
        {
            return this.ctx.Employee.FromSql(ArcadianEmployeeQuery);
        }
    }
}
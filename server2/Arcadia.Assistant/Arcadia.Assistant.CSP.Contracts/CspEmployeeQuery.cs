namespace Arcadia.Assistant.CSP.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    using Models;

    public class CspEmployeeQuery
    {
        private const int ArcadyKhotinEmployeeId = 145;
        private readonly CspConfiguration cspConfiguration;

        private readonly ArcadiaCspContext ctx;

        public CspEmployeeQuery(ArcadiaCspContext ctx, CspConfiguration cspConfiguration)
        {
            this.ctx = ctx;
            this.cspConfiguration = cspConfiguration;
        }

        public async Task<IQueryable<Employee>> Get()
        {
            var employees = await this.ctx.GetEmployeesAsync();
            return employees
                .Where(x => x.FiringDate == null && !x.IsDelete && x.CompanyId == this.cspConfiguration.CompanyId ||
                    x.Id == ArcadyKhotinEmployeeId)
                .AsQueryable();
        }
    }
}
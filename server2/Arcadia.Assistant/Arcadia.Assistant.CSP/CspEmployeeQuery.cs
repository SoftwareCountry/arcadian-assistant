﻿namespace Arcadia.Assistant.CSP
{
    using System.Linq;

    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeeQuery
    {
        private const int ArcadyKhotinEmployeeId = 145;

        private readonly ArcadiaCspContext ctx;
        private readonly CspConfiguration cspConfiguration;

        public CspEmployeeQuery(ArcadiaCspContext ctx, CspConfiguration cspConfiguration)
        {
            this.ctx = ctx;
            this.cspConfiguration = cspConfiguration;
        }

        public IQueryable<Employee> Get(bool includeFired = false)
        {
            var query = this.ctx.Employees
                .AsNoTracking();

            return includeFired
                ? query.Where(x => !x.IsDelete && x.CompanyId == this.cspConfiguration.CompanyId || x.Id == ArcadyKhotinEmployeeId)
                : query.Where(x => x.FiringDate == null && !x.IsDelete && x.CompanyId == this.cspConfiguration.CompanyId || x.Id == ArcadyKhotinEmployeeId);
        }
    }
}
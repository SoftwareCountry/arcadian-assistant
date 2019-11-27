﻿namespace Arcadia.Assistant.CSP
{
    using Arcadia.Assistant.CSP.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

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

        public IQueryable<Employee> Get()
        {
            return this.ctx.Employees
                .AsNoTracking()
                .Where(x => x.FiringDate == null && !x.IsDelete && x.CompanyId == this.cspConfiguration.CompanyId || x.Id == ArcadyKhotinEmployeeId);
        }
    }
}
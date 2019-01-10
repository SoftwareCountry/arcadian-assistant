namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Arcadia.Assistant.CSP.Model;

    public class EmployeesQueryExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public EmployeesQueryExecutor(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<CspEmployeeRecord>> Fetch()
        {
            var cspRecords = await this.GetCspRecords();
            return cspRecords;
        }

        private async Task<List<CspEmployeeRecord>> GetCspRecords()
        {
            using (var ctx = this.contextFactory())
            {
                var arcEmployees = new CspEmployeeQuery(ctx)
                    .Get()
                    .Where(x => x.Email != null);
                return await arcEmployees.Select(
                    x =>
                        new CspEmployeeRecord
                        {
                            Id = x.Id.ToString(),
                            Email = x.Email
                        }).ToListAsync();
            }
        }

        public class CspEmployeeRecord
        {
            public string Id { get; set; }

            public string Email { get; set; }
        }
    }
}
namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class EmployeeInfoQuery : IEmployeeInfoQuery
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public EmployeeInfoQuery(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<EmployeeDemographics> GetEmployeeDemographics(string employeeId)
        {
            using (var context = this.contextFactory())
            {
                return await context
                    .Employee
                    .Where(x => x.Sid.GetValueOrDefault().ToString() == employeeId)
                    .Select(x => new EmployeeDemographics(employeeId)
                        {
                            BirthDate = x.Birthday,
                            Email = x.Email,
                            HireDate = x.HiringDate,
                            Name = $"{x.LastName}, {x.FirstName} {x.MiddleName}".Trim(),
                            PhotoBase64 = Convert.ToBase64String(x.Image),
                            Sex = x.Gender == "M"
                                ? Sex.Male
                                : x.Gender == "F"
                                    ? Sex.Female
                                    : Sex.Undefined
                        })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
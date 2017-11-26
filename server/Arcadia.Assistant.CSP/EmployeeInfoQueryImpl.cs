namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class EmployeeInfoQueryImpl : EmployeeInfoQuery
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public EmployeeInfoQueryImpl(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected override async Task<OrganizationRequests.RequestEmployeeInfo.Success> GetEmployeeDemographics(string employeeId)
        {
            var dbId = long.Parse(employeeId);

            using (var context = this.contextFactory())
            {
                var employeeInfo = await context
                    .Employee
                    .Where(x => x.Id == dbId)
                    .Select(x => new EmployeeInfo(employeeId)
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

                return new OrganizationRequests.RequestEmployeeInfo.Success(employeeInfo);
            }
        }
    }
}
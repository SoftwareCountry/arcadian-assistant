namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeesInfoStorage : EmployeesInfoStorage
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public CspEmployeesInfoStorage(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected override async Task<LoadAllEmployees.Response> GetAllEmployees()
        {
            using (var context = this.contextFactory())
            {
                var employees = await context
                    .Employee
                    .Select(x => new EmployeeInfo(x.Id.ToString())
                    {
                        BirthDate = x.Birthday,
                        Email = x.Email,
                        HireDate = x.HiringDate,
                        DepartmentId = x.DepartmentId.HasValue ? x.DepartmentId.Value.ToString() : null,
                        Name = $"{x.LastName}, {x.FirstName} {x.MiddleName}".Trim(),
                        //PhotoBase64 = x.Image == null ? null : Convert.ToBase64String(x.Image),
                        Sex = x.Gender == "M"
                                ? Sex.Male
                                : x.Gender == "F"
                                    ? Sex.Female
                                    : Sex.Undefined
                    })
                    .ToListAsync();
                return new LoadAllEmployees.Response(employees);
            }
        }
    }
}
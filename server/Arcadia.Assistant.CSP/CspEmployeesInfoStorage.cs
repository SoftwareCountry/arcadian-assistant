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

        protected override async Task<LoadDepartmentsEmployees.Response> GetDepartmentEmployees(string departmentId)
        {
            var dbId = long.Parse(departmentId);

            using (var context = this.contextFactory())
            {
                try
                {
                    var employees = await context
                        .Employee
                        .Where(x => x.DepartmentId == dbId)
                        .Select(x => new EmployeeInfo(x.Id.ToString())
                        {
                            BirthDate = x.Birthday,
                            Email = x.Email,
                            HireDate = x.HiringDate,
                            Name = $"{x.LastName}, {x.FirstName} {x.MiddleName}".Trim(),
                            PhotoBase64 = x.Image == null ? null : Convert.ToBase64String(x.Image),
                            Sex = x.Gender == "M"
                                    ? Sex.Male
                                    : x.Gender == "F"
                                        ? Sex.Female
                                        : Sex.Undefined
                        })
                        .ToListAsync();
                    return new LoadDepartmentsEmployees.Response(employees);
                }
                catch (Exception e)
                {  }

                return new LoadDepartmentsEmployees.Response(new List<EmployeeInfo>());
            }
        }
    }
}
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

        public const string ArcadianEmployeeQuery = "SELECT * FROM dbo.ArcadianEmployee";

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
                    .FromSql(ArcadianEmployeeQuery)
                    .Select(x => new EmployeeStoredInformation(
                        new EmployeeMetadata(x.Id.ToString(), $"{x.LastName} {x.FirstName}".Trim(), x.Email)
                        {
                            BirthDate = x.Birthday,
                            HireDate = x.HiringDate,
                            FireDate = x.FiringDate,
                            MobilePhone = x.MobilePhone,
                            RoomNumber = x.RoomNumber,
                            Position = x.Position.Title,
                            Sid = x.Sid.HasValue ? x.Sid.Value.ToString() : null,
                            DepartmentId = x.DepartmentId.HasValue ? x.DepartmentId.Value.ToString() : null,
                            Sex = x.Gender == "M"
                                    ? Sex.Male
                                    : x.Gender == "F"
                                        ? Sex.Female
                                        : Sex.Undefined
                        })
                    {
                        Photo = x.Id != 777 ? x.Image : null, //TODO: deal with the bad image
                    })
                    .ToListAsync();
                return new LoadAllEmployees.Response(employees);
            }
        }
    }
}
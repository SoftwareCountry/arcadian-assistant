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
                    .FromSql("SELECT * FROM dbo.ArcadianEmployee")
                    .Select(x => new EmployeeStoredInformation(x.Id.ToString())
                    {
                        BirthDate = x.Birthday,
                        Email = x.Email,
                        HireDate = x.HiringDate,
                        FireDate = x.FiringDate,
                        Photo = x.Image,
                        Phone = x.MobilePhone,
                        RoomNumber = x.RoomNumber,
                        Position = x.Position.Title,
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
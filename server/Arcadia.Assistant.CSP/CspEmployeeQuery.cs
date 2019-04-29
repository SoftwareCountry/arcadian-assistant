namespace Arcadia.Assistant.CSP
{
    using System.Linq;

    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeeQuery
    {
        private readonly ArcadiaCspContext ctx;

        public const string ArcadianEmployeeQuery = @"
            SELECT 
                Id, FirstName, MiddleName, LastName, FirstNameRus, MiddleNameRus, LastNameRus, LoginName, SID, Birthday, BusinessPhone, HomePhone, Email,
                DepartmentId, PositionId, HiringDate, FiringDate, IsWorking, Description, IsPartTime, Image, CompanyId, IsInStaff, IntrabaseId, Gender,
                ClockNumber, RoomNumber, BusinessCountry, BusinessZIP, BusinessCity, BusinessStreet, BusinessStreet2, BusinessStreet3, BusinessPhone2,
                HomeCountry, HomeZIP, HomeCity, HomeStreet, HomeStreet2, HomeStreet3, MobilePhone, ProbationEnd, WeekHours, PartTime, IsDelete
            FROM dbo.Employee
            WHERE (FiringDate IS NULL) AND (IsDelete <> 1) AND (CompanyId = 154) OR (Id = 145)";

        public CspEmployeeQuery(ArcadiaCspContext ctx)
        {
            this.ctx = ctx;
        }

        public IQueryable<Employee> Get()
        {
            return this.ctx.Employee.FromSql(ArcadianEmployeeQuery);
        }
    }
}
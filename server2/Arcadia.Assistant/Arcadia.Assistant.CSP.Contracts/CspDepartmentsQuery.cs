namespace Arcadia.Assistant.CSP.Contracts
{
    using System.Linq;

    using Models;

    public class CspDepartmentsQuery
    {
        private readonly CspConfiguration configuration;
        private readonly ArcadiaCspContext cspContext;

        public CspDepartmentsQuery(CspConfiguration configuration, ArcadiaCspContext cspContext)
        {
            this.configuration = configuration;
            this.cspContext = cspContext;
        }

        public IQueryable<DepartmentWithPeopleCount> Get()
        {
            var arcEmployees = new CspEmployeeQuery(this.cspContext, this.configuration).Get();

            var employeeByDepCounts = arcEmployees
                .Where(x => x.DepartmentId != null)
                .GroupBy(x => x.DepartmentId)
                .Select(x => new { DepartmentId = x.Key, EmployeesCount = x.Count() });

            var organizationDepartments = this
                .cspContext
                .Departments
                .Where(x => x.IsDelete != true && x.CompanyId == this.configuration.CompanyId)
                .AsQueryable();

            var chiefs = organizationDepartments
                .Join(arcEmployees, d => d.ChiefId, e => e.Id, (d,e) => new { DepartmentId = d.Id, e.Id });

            var allDepartments = organizationDepartments
                .GroupJoin(employeeByDepCounts, d => d.Id, e => e.DepartmentId, (d, e) => 
                    new DepartmentWithPeopleCount()
                    {
                        ActualChiefId = d.ChiefId,
                        Department = d,
                        PeopleCount = e.Select(x => x.EmployeesCount).DefaultIfEmpty(0).First()
                    })
                .GroupJoin(chiefs, d => d.Department.Id, e => e.DepartmentId, (d, e) =>
                    new DepartmentWithPeopleCount()
                    {
                        ActualChiefId = e.Select(x => (int?)x.Id).DefaultIfEmpty(null).First(),
                        Department = d.Department,
                        PeopleCount = d.PeopleCount
                    });

            return allDepartments;
        }

        public class DepartmentWithPeopleCount
        {
            public Department Department { get; set; } = new Department();

            public int? ActualChiefId { get; set; }

            public int PeopleCount { get; set; }
        }
    }
}
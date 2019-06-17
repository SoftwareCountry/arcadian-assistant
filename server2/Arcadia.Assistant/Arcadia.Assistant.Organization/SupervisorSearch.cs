namespace Arcadia.Assistant.Organization
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    public class SupervisorSearch : ISupervisorSearch
    {
        private readonly IEmployees employees;
        private readonly IOrganizationDepartments organizationDepartments;

        public SupervisorSearch(IEmployees employees, IOrganizationDepartments organizationDepartments)
        {
            this.employees = employees;
            this.organizationDepartments = organizationDepartments;
        }

        public async Task<EmployeeMetadata> FindAsync(string employeeId, CancellationToken cancellationToken)
        {
            var target = await this.employees.FindEmployeeAsync(employeeId, cancellationToken);
            if (target == null)
            {
                return null;
            }

            var departments = await this.organizationDepartments.GetAllAsync(cancellationToken);

            var candidateDepartment = target.DepartmentId;
            var candidate = target;

            while (true)
            {
                var department = departments.FirstOrDefault(x => x.DepartmentId == candidateDepartment);
                if (department == null)
                {
                    // if supposed supervisor department is not found, we have no choice but to finish search
                    return candidate;
                }

                var chief = await this.employees.FindEmployeeAsync(department.ChiefId, cancellationToken);

                if (chief != null && chief.EmployeeId != candidate.EmployeeId || department.IsHeadDepartment)
                {
                    // we found the boss
                    return chief;
                }

                if (chief != null)
                {
                    candidate = chief;
                }

                //we go one level up
                candidateDepartment = department.ParentDepartmentId;
            }
        }
    }
}
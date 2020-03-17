namespace Arcadia.Assistant.Permissions.Contracts
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    public class PermissionsEntryQuery
    {
        private readonly IPermissions permissions;

        private readonly IEmployees employees;

        public PermissionsEntryQuery(IPermissions permissions, IEmployees employees)
        {
            this.permissions = permissions;
            this.employees = employees;
        }

        public async Task<(EmployeeId? employeeId, EmployeePermissionsEntry permissions)> ExecuteAsync(UserIdentity identity, EmployeeId employeeId, CancellationToken cancellationToken)
        {
            var targetEmployee = await this.employees.FindEmployeeAsync(employeeId, cancellationToken);
            var userEmployee =
                (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(identity.Value),
                    cancellationToken)).FirstOrDefault()?.EmployeeId;
                
            if (targetEmployee == null)
            {
                return (userEmployee, EmployeePermissionsEntry.None);
            }

            var employeePermissions = await this.permissions.GetPermissionsAsync(identity, cancellationToken);
            return (userEmployee, employeePermissions.GetPermissions(targetEmployee));
        }
    }
}
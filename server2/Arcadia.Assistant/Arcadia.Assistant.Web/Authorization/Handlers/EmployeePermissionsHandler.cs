namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;

    using Permissions.Contracts;

    using Requirements;

    public class EmployeePermissionsHandler : AuthorizationHandler<EmployeePermissionsRequirement, EmployeeId>
    {
        private readonly IEmployees employees;
        private readonly IPermissions permissionsLoader;

        public EmployeePermissionsHandler(IPermissions permissionsLoader, IEmployees employees)
        {
            this.permissionsLoader = permissionsLoader;
            this.employees = employees;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, EmployeePermissionsRequirement requirement, EmployeeId resource)
        {
            if (context.User.Identity.Name == null)
            {
                return;
            }

            var allPermissions =
                await this.permissionsLoader.GetPermissionsAsync(new UserIdentity(context.User.Identity.Name),
                    CancellationToken.None);
            var employee = await this.employees.FindEmployeeAsync(resource, CancellationToken.None);
            if (employee == null)
            {
                return;
            }

            var employeePermissions = allPermissions.GetPermissions(employee);

            if (employeePermissions.HasFlag(requirement.RequiredPermissions))
            {
                context.Succeed(requirement);
            }
        }
    }
}
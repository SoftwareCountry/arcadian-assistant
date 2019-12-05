namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Authorization.Requirements;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;

    using Permissions.Contracts;

    public class EmployeePermissionsHandler : AuthorizationHandler<RequiredEmployeePermissions, EmployeeMetadata>
    {
        private readonly IPermissions permissionsLoader;

        public EmployeePermissionsHandler(IPermissions permissionsLoader)
        {
            this.permissionsLoader = permissionsLoader;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiredEmployeePermissions requirement, EmployeeMetadata resource)
        {
            if (context.User.Identity.Name == null)
            {
                return;
            }

            var allPermissions = await this.permissionsLoader.GetPermissionsAsync(context.User.Identity.Name, CancellationToken.None);
            var employeePermissions = allPermissions.GetPermissions(resource);

            if (employeePermissions.HasFlag(requirement.RequiredPermissions))
            {
                context.Succeed(requirement);
            }
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading.Tasks;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Web.Authorization.Requirements;

    using Microsoft.AspNetCore.Authorization;

    public class EmployeePermissionsHandler : AuthorizationHandler<RequiredEmployeePermissions, EmployeeContainer>
    {
        private readonly IPermissionsLoader permissionsLoader;

        public EmployeePermissionsHandler(IPermissionsLoader permissionsLoader)
        {
            this.permissionsLoader = permissionsLoader;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiredEmployeePermissions requirement, EmployeeContainer resource)
        {
            var allPermissions = await this.permissionsLoader.LoadAsync(context.User);
            var employeePermissions = allPermissions.GetPermissions(resource);

            if (requirement.HasPermissions(employeePermissions))
            {
                context.Succeed(requirement);
            }
        }
    }
}
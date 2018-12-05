namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Users;

    public class CanApproveOnBehalfPermissionHandler : AuthorizationHandler<CanApproveOnBehalfRequirement, EmployeeContainer>
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;

        public CanApproveOnBehalfPermissionHandler(IUserEmployeeSearch userEmployeeSearch)
        {
            this.userEmployeeSearch = userEmployeeSearch;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanApproveOnBehalfRequirement requirement, EmployeeContainer resource)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(context.User, CancellationToken.None);
            if (employee.Metadata.EmployeeId == resource.Metadata.EmployeeId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
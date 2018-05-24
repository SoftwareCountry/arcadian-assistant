namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;

    public class UserIsEmployeeHandler : AuthorizationHandler<UserIsEmployeeRequirement>
    {
        private readonly IUserEmployeeSearch search;

        public UserIsEmployeeHandler(IUserEmployeeSearch search)
        {
            this.search = search;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsEmployeeRequirement requirement)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Name);
            if (email == null)
            {
                return;
            }

            var employee = await this.search.FindOrDefaultAsync(context.User, CancellationToken.None);
            if (employee != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;

    using NLog;

    public class UserIsEmployeeHandler : AuthorizationHandler<UserIsEmployeeRequirement>
    {
        private readonly IUserEmployeeSearch search;

        private readonly ILogger logger = LogManager.GetLogger("Auth");

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
            else
            {
                this.logger.Trace($"Employee not found in the database for the user with identity {context.User.Identity.Name}");
            }
        }
    }
}
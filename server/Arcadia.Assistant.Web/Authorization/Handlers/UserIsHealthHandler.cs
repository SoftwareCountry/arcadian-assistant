namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Requirements;

    public class UserIsHealthHandler : AuthorizationHandler<UserIsHealthRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsHealthRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
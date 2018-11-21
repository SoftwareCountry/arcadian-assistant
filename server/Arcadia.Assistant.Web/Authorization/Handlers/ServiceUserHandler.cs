namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Requirements;

    public class ServiceUserHandler : AuthorizationHandler<ServiceUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ServiceUserRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
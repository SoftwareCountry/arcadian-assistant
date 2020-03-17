namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;

    using Requirements;

    public class UserIsEmployeeHandler : AuthorizationHandler<UserIsEmployeeRequirement>
    {
        private readonly IEmployees employees;

        //private readonly ILogger logger = LogManager.GetLogger("Auth");

        public UserIsEmployeeHandler(IEmployees employees)
        {
            this.employees = employees;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsEmployeeRequirement requirement)
        {
            //this.logger.Trace($"User is employee authorization check started for user {context.User.Identity.Name}");

            var email = context.User.FindFirstValue(ClaimTypes.Name);
            if (email == null)
            {
                return;
            }

            var employee = (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(email), CancellationToken.None))
                .FirstOrDefault();
            //this.logger.Trace($"Employee is loaded from the database for user {context.User.Identity.Name}");

            if (employee != null)
            {
                context.Succeed(requirement);
            }
            else
            {
                // this.logger.Trace($"Employee not found in the database for the user with identity {context.User.Identity.Name}");
            }
        }
    }
}
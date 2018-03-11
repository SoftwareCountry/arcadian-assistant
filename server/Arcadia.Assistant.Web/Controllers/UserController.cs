namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public UserController(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder, ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken token)
        {
            string email;
            var defaultEmail = "alexander.shevnin@arcadia.spb.ru";

            //TODO: temp assignment, before authentication works
            if (!this.User.Identity.IsAuthenticated)
            {
                email = defaultEmail;
            }
            else
            {
                email = this.User.Identity.Name;
            }

            //TODO: GET RID OF THAT COPY-PASTE!
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get("organization"));
            var query = EmployeesQuery.Create().WithEmail(email);
            var response = await organization.Ask<EmployeesQuery.Response>(query, this.timeoutSettings.Timeout, token);
            if (!response.Employees.Any())
            {
                //TODO: temp measure, before all users are in database
                query = EmployeesQuery.Create().WithEmail(defaultEmail);
                response = await organization.Ask<EmployeesQuery.Response>(query, this.timeoutSettings.Timeout, token);
            }

            var userEmployee = response.Employees.SingleOrDefault();
            if (userEmployee == null)
            {
                return this.Forbid();
            }

            return this.Ok(new UserModel()
                {
                    EmployeeId = userEmployee.Metadata.EmployeeId,
                    Username = userEmployee.Metadata.Name
                });
        }
    }
}
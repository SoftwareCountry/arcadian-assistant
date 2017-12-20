namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;

    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        public EmployeesController(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
        }

        [Route("{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeInfo), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string employeeId, CancellationToken token)
        {
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get("organization"));
            var response = await organization.Ask<EmployeesQuery.Response>(new EmployeesQuery().WithId(employeeId), TimeSpan.FromSeconds(30), token);

            if (response.Employees.Count == 0)
            {
                return this.NotFound();
            }

            return this.Ok(response.Employees.Select(x => x.EmployeeInfo).Single());
        }
    }
}
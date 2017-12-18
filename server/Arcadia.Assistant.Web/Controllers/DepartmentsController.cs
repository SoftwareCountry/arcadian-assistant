namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;

    using Microsoft.AspNetCore.Mvc;

    [Route("/api/departments")]
    public class DepartmentsController : Controller
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        public DepartmentsController(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
        }

        [Route("")]
        [ProducesResponseType(typeof(Department[]), 200)]
        public async Task<IActionResult> All(CancellationToken token)
        {
            var employees = this.actorSystem.ActorSelection(this.pathsBuilder.Get("organization"));
            var response = await employees.Ask<FindDepartments.Response>(new FindDepartments(), TimeSpan.FromSeconds(30), token);
            return this.Ok(response.Departments.Select(x => x.Department).ToArray());
        }
    }
}
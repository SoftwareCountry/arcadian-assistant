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
    using Arcadia.Assistant.Web.Configuration;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/api/departments")]
    public class DepartmentsController : Controller
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public DepartmentsController(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder, ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentInfo[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> All(CancellationToken token)
        {
            var departments = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Organization));
            var response = await departments.Ask<DepartmentsQuery.Response>(new DepartmentsQuery(), this.timeoutSettings.Timeout, token);
            return this.Ok(response.Departments.Select(x => x.Department).OrderBy(x => x.DepartmentId).ToArray());
        }

        [Route("{departmentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string departmentId, CancellationToken token)
        {
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Organization));
            var response = await organization.Ask<DepartmentsQuery.Response>(new DepartmentsQuery().WithId(departmentId), this.timeoutSettings.Timeout, token);
            return this.Ok(response.Departments.Select(x => x.Department).FirstOrDefault());
        }
    }
}
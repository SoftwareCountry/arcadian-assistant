namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;
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

        [Route("")]
        [ProducesResponseType(typeof(EmployeeInfo[]), 200)]
        public async Task<IActionResult> All(CancellationToken token)
        {
            return this.Ok();
        }

        [Route("{id}")]
        [ProducesResponseType(typeof(EmployeeInfo), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id, CancellationToken token)
        {
            var employees = this.actorSystem.ActorSelection(this.pathsBuilder.Get("organization"));
            var response = await employees.Ask(new OrganizationRequests.RequestEmployeeInfo(id), TimeSpan.FromSeconds(30), token);

            switch (response)
            {
                case OrganizationRequests.RequestEmployeeInfo.Success value:
                    return this.Ok(value.EmployeeInfo);
                case OrganizationRequests.RequestEmployeeInfo.EmployeeNotFound _:
                    return this.NotFound();
                default:
                    return this.StatusCode(500);
            }
        }
    }
}
namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Organization;
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

        [Route("demographics")]
        // GET
        public async Task<IActionResult> Index()
        {
            var employees = this.actorSystem.ActorSelection(this.pathsBuilder.Get("employees"));
            var response = await employees.Ask<EmployeeDemographics>(new RequestDemographics(this.User.Identity.Name));
            return this.Ok(response);
            //return response;
        }
    }
}
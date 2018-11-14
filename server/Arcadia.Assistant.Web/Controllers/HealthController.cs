namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class HealthController : Controller
    {
        [Route("/api/ping/health")]
        [HttpGet]
        [ProducesResponseType(typeof(ApplicationHealthModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationHealth(CancellationToken cancellationToken)
        {
            return this.Ok(new ApplicationHealthModel());
        }
    }
}
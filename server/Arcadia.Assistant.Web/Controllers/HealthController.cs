namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Health;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class HealthController : Controller
    {
        private readonly IHealthService healthService;

        public HealthController(IHealthService healthService)
        {
            this.healthService = healthService;
        }

        [Route("/api/ping/health")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationHealthModelEntry>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationHealth(CancellationToken cancellationToken)
        {
            var healthStateDict = await this.healthService.GetHealthState(cancellationToken);

            var result = healthStateDict
                .Select(kvp => new ApplicationHealthModelEntry(kvp.Key, kvp.Value.Value, kvp.Value.Details))
                .ToList();

            return this.Ok(result);
        }
    }
}
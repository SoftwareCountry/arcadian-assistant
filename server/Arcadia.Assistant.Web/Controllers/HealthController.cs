namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authorization;
    using Health;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Authorize(Policies.ServiceEndpoint)]
    public class HealthController : Controller
    {
        private readonly IHealthService healthService;

        public HealthController(IHealthService healthService)
        {
            this.healthService = healthService;
        }

        [Route("/api/health")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationHealthModelEntry>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<ApplicationHealthModelEntry>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetApplicationHealth(CancellationToken cancellationToken)
        {
            var healthStateDict = await this.healthService.GetHealthState(cancellationToken);

            var result = healthStateDict
                .Select(kvp => new ApplicationHealthModelEntry(kvp.Key, kvp.Value.Value, kvp.Value.Details))
                .ToList();

            if (result.All(x => x.StateValue))
            {
                return this.Ok(result);
            }

            return this.StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
    }
}
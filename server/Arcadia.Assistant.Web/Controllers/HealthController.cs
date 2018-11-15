namespace Arcadia.Assistant.Web.Controllers
{
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
        [ProducesResponseType(typeof(ApplicationHealthModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationHealth(CancellationToken cancellationToken)
        {
            var tasks = new[]
            {
                this.healthService.GetIsServerAlive(cancellationToken),
                this.healthService.GetIs1CAlive(cancellationToken),
                this.healthService.GetIsDatabaseAlive(cancellationToken)
            };

            var result = await Task.WhenAll(tasks);

            return this.Ok(new ApplicationHealthModel
            {
                IsServerAlive = result[0],
                Is1CAlive = result[1],
                IsDatabaseAlive = result[2]
            });
        }
    }
}
namespace Arcadia.Assistant.Web.Controllers.Calendar
{
    using Employees.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Calendar;
    using PendingActions.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/pending-requests")]
    [Authorize]
    public class PendingRequestsController : Controller
    {
        private readonly IPendingActions pendingActions;

        public PendingRequestsController(IPendingActions pendingActions)
        {
            this.pendingActions = pendingActions;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<CalendarEventsWithIdByEmployeeModel>> GetPendingRequests(CancellationToken cancellationToken)
        {
            var id = new EmployeeId(144);

            var requests = await this.pendingActions.GetPendingRequestsAsync(id, cancellationToken);

            return this.Ok(requests);
        }
    }
}
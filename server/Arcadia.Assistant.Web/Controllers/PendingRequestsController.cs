namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models.Calendar;
    using Arcadia.Assistant.Web.Users;

    [Route("api/pending-requests")]
    [Authorize(Policies.UserIsEmployee)]
    public class PendingRequestsController : Controller
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;

        private readonly ITimeoutSettings timeoutSettings;

        public PendingRequestsController(
            IUserEmployeeSearch userEmployeeSearch,
            ITimeoutSettings timeoutSettings)
        {
            this.userEmployeeSearch = userEmployeeSearch;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(CalendarEventsWithIdByEmployeeModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingRequestsForUser(string fromUser, CancellationToken token)
        {
            var user = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);
            if (user == null)
            {
                return this.Forbid();
            }

            var calendarEvents = await user.Calendar.PendingActionsActor.Ask<GetEmployeePendingActions.Response>(
                GetEmployeePendingActions.Instance, this.timeoutSettings.Timeout, token);

            var modelValues = calendarEvents
                .PendingActions
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(ev => new CalendarEventsWithIdModel(ev.EventId, ev.Type, ev.Dates, ev.Status)));

            return this.Ok(new CalendarEventsWithIdByEmployeeModel(modelValues));
        }
    }
}
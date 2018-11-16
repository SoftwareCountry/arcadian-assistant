namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models.Calendar;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/pending-requests")]
    [Authorize]
    public class PendingRequestsController : Controller
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;

        private readonly ActorPathsBuilder pathBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        private readonly IActorRefFactory actorsFactory;

        public PendingRequestsController(
            IUserEmployeeSearch userEmployeeSearch,
            ActorPathsBuilder pathBuilder,
            ITimeoutSettings timeoutSettings,
            IActorRefFactory actorsFactory)
        {
            this.userEmployeeSearch = userEmployeeSearch;
            this.pathBuilder = pathBuilder;
            this.timeoutSettings = timeoutSettings;
            this.actorsFactory = actorsFactory;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(CalendarEventsWithIdByEmployeeModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingRequestsForUser(CancellationToken token)
        {
            var user = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);
            if (user == null)
            {
                return this.Forbid();
            }

            var actor = this.actorsFactory.ActorOf(PendingActionsRequest.CreateProps(this.pathBuilder));
            var calendarEvents = await actor.Ask<PendingActionsRequest.GetPendingActions.Response>(
                // Further DependentDepartmentsPendingActions should be retrieved from user preferences
                new PendingActionsRequest.GetPendingActions(user.Metadata, PendingActionsRequest.DependentDepartmentsPendingActions.HeadsOnly),
                this.timeoutSettings.Timeout,
                token);

            var modelValues = calendarEvents
                .EventsByEmployeeId
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Select(ev => new CalendarEventsWithIdModel(ev.EventId, ev.Type, ev.Dates, ev.Status)));

            return this.Ok(new CalendarEventsWithIdByEmployeeModel(modelValues));
        }
    }
}
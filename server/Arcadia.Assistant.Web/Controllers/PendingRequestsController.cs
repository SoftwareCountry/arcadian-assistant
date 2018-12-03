namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models.Calendar;
    using Arcadia.Assistant.Web.Users;
    using Arcadia.Assistant.Web.UserPreferences;

    [Route("api/pending-requests")]
    [Authorize(Policies.UserIsEmployee)]
    public class PendingRequestsController : Controller
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;

        private readonly ActorPathsBuilder pathBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        private readonly IActorRefFactory actorsFactory;
        private readonly IUserPreferencesService userPreferencesService;

        public PendingRequestsController(
            IUserEmployeeSearch userEmployeeSearch,
            ActorPathsBuilder pathBuilder,
            ITimeoutSettings timeoutSettings,
            IActorRefFactory actorsFactory,
            IUserPreferencesService userPreferencesService)
        {
            this.userEmployeeSearch = userEmployeeSearch;
            this.pathBuilder = pathBuilder;
            this.timeoutSettings = timeoutSettings;
            this.actorsFactory = actorsFactory;
            this.userPreferencesService = userPreferencesService;
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

            var userPreferences = await this.userPreferencesService.GetUserPreferences(this.User.Identity.Name, token);

            var actor = this.actorsFactory.ActorOf(PendingActionsRequest.CreateProps(this.pathBuilder));
            var calendarEvents = await actor.Ask<PendingActionsRequest.GetPendingActions.Response>(
                new PendingActionsRequest.GetPendingActions(user, userPreferences.DependentDepartmentsPendingActions),
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
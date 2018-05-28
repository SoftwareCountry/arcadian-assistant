using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Web.Authorization;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    [Route("/api/metadata/calendar-workflow")]
    [Authorize(Policies.UserIsEmployee)]
    public class CalendarEventsWorkflowMetadataController : Controller
    {
        [Route("types")]
        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public IActionResult GetTypes()
        {
            return this.Ok(CalendarEventTypes.All);
        }

        [Route("types/{type}/statuses")]
        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetStatusesForType(string type)
        {
            var statuses = new CalendarEventStatuses().AllForType(type);
            if (statuses.Length != 0)
            {
                return this.Ok(statuses);
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
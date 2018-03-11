using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using Arcadia.Assistant.Calendar.Abstractions;

    using Microsoft.AspNetCore.Http;

    [Route("/api/metadata/calendar-workflow")]
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
        public IActionResult GetStatusesForType(string type)
        {
            return this.Ok(CalendarEventStatuses.All);
        }
    }
}
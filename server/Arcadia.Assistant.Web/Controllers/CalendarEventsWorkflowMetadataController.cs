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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetStatusesForType(string type)
        {
            if (string.Equals(type, CalendarEventTypes.Vacation))
            {
                return this.Ok(VacationStatuses.All);
            }

            if (string.Equals(type, CalendarEventTypes.Dayoff) || string.Equals(type, CalendarEventTypes.Workout))
            {
                return this.Ok(WorkHoursChangeStatuses.All);
            }

            if (string.Equals(type, CalendarEventTypes.Sickleave))
            {
                return this.Ok(SickLeaveStatuses.All);
            }

            return this.NotFound();
        }
    }
}
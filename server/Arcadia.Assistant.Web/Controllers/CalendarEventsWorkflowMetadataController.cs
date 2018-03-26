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
            switch (type)
            {
                case CalendarEventTypes.Vacation:
                    return this.Ok(VacationStatuses.All);

                case CalendarEventTypes.Workout:
                case CalendarEventTypes.Dayoff:
                    return this.Ok(WorkHoursChangeStatuses.All);

                case CalendarEventTypes.Sickleave:
                    return this.Ok(SickLeaveStatuses.All);

                default:
                    return this.NotFound();
            }
        }
    }
}
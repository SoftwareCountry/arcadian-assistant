using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    using Models.Calendar;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/")]
    [Authorize]
    public class CalendarEventsController : Controller
    {
        private readonly IWorkHoursCredit workHoursCredit;

        public CalendarEventsController(IWorkHoursCredit workHoursCredit)
        {
            this.workHoursCredit = workHoursCredit;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventsWithIdModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(string employeeId, CancellationToken token)
        {
            var workHoursEvents = await this.workHoursCredit.GetCalendarEventsAsync(employeeId, token);
            var allEvents = workHoursEvents.Select(x =>
                new CalendarEventsWithIdModel(
                    x.ChangeId.ToString(),
                    x.ChangeType == WorkHoursChangeType.Dayoff ? CalendarEventTypes.Dayoff : CalendarEventTypes.Workout,
                    new DatesPeriodModel()
                    {
                        StartDate = x.Date,
                        EndDate = x.Date,
                        StartWorkingHour = x.DayPart == DayPart.SecondHalf ? 4 : 0,
                        FinishWorkingHour = x.DayPart == DayPart.FirstHalf ? 4 : 8
                    },
                    x.Status));

            return this.Ok(allEvents);
        }


        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(typeof(CalendarEventsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string employeeId, string eventId, CancellationToken token)
        {
            return this.Ok();
        }
    }
}
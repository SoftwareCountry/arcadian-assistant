namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Arcadia.Assistant.Web.Models.Calendar;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/api/employees/{employeeId}/events/")]
    public class CalendarEventsController : Controller
    {
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventsWithIdModel>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string employeeId)
        {
            var mock = new CalendarEventsWithIdModel(Guid.NewGuid().ToString(), DateTime.Today.AddDays(-4), DateTime.Today.AddDays(1));

            return this.Ok(new[] { mock });
        }

        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(typeof(CalendarEventsModel), StatusCodes.Status200OK)]
        public IActionResult Get(string employeeId, string eventId)
        {
            return this.Ok(new CalendarEventsModel(DateTime.Now.AddDays(-7), DateTime.Now.AddDays(4))
            {
                Status = CalendarEventStatus.Approved
            });
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CalendarEventsWithIdModel), StatusCodes.Status201Created)]
        public IActionResult Create(string employeeId, [FromBody] CalendarEventsModel model)
        {
            var copy = new CalendarEventsWithIdModel
            {
                CalendarEventId = Guid.NewGuid().ToString(),
                Dates = model.Dates,
                Status = model.Status
            };

            return this.AcceptedAtAction(nameof(this.Get), new { eventId = copy.CalendarEventId }, copy);
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(string employeeId, string eventId, [FromBody] CalendarEventsModel model)
        {
            return this.NoContent();
        }
    }
}
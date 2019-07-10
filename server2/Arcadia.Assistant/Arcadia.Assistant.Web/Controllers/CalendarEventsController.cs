using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    using Models.Calendar;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/")]
    [Authorize]
    public class CalendarEventsController : Controller
    {
        private readonly IWorkHoursCredit workHoursCredit;
        private readonly IEmployees employees;
        private readonly WorkHoursConverter workHoursConverter = new WorkHoursConverter();

        public CalendarEventsController(IWorkHoursCredit workHoursCredit, IEmployees employees)
        {
            this.workHoursCredit = workHoursCredit;
            this.employees = employees;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventWithIdModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(string employeeId, CancellationToken token)
        {
            var workHoursEvents = await this.workHoursCredit.GetCalendarEventsAsync(employeeId, token);
            var allEvents = workHoursEvents.Select(this.workHoursConverter.ToCalendarEventWithId);

            return this.Ok(allEvents);
        }


        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(typeof(CalendarEventModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string employeeId, Guid eventId, CancellationToken token)
        {
            var change = await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, token);
            if (change == null)
            {
                return this.NotFound();
            }
            return this.Ok(this.workHoursConverter.ToCalendarEvent(change));
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CalendarEventWithIdModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(string employeeId, [FromBody] CalendarEventModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            switch (model.Type)
            {
                case CalendarEventTypes.Dayoff:
                case CalendarEventTypes.Workout:
                    var type = model.Type == CalendarEventTypes.Dayoff ? WorkHoursChangeType.Dayoff : WorkHoursChangeType.Workout;
                    var dayPart = this.workHoursConverter.GetDayPart(model.Dates);
                    var change = await this.workHoursCredit.RequestChangeAsync(employeeId, type, model.Dates.StartDate, dayPart);

                    return this.CreatedAtAction(nameof(this.Get), new { employeeId, eventId = change.ChangeId }, change);
                default:
                    return this.BadRequest("Unknown model type");
            }
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(string employeeId, Guid eventId, [FromBody] CalendarEventModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var changedBy = (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity.Name), CancellationToken.None)).First();

            switch (model.Type)
            {
                case CalendarEventTypes.Dayoff:
                case CalendarEventTypes.Workout:
                    return await this.UpdateWorkHoursRequest(employeeId, eventId, model, changedBy);
            }

            return this.Ok();
        }

        private async Task<IActionResult> UpdateWorkHoursRequest(string employeeId, Guid eventId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            //TODO: possibly add a check for date changes
            if (model.Status != existingEvent.Status)
            {
                //TODO: add authorization

                if (model.Status == WorkHoursChangeStatuses.Cancelled)
                {
                    await this.workHoursCredit.CancelRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }

                if (model.Status == WorkHoursChangeStatuses.Rejected)
                {
                    await this.workHoursCredit.RejectRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }
            }

            return this.NoContent();
        }
    }
}
namespace Arcadia.Assistant.Web.Controllers.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models.Calendar;

    using NSwag.Annotations;

    using SickLeaves.Contracts;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/")]
    [Authorize]
    public class CalendarEventsController : Controller
    {
        private readonly IWorkHoursCredit workHoursCredit;
        private readonly ISickLeaves sickLeaves;
        private readonly IEmployees employees;
        private readonly WorkHoursConverter workHoursConverter = new WorkHoursConverter();
        private readonly SickLeavesConverter sickLeavesConverter = new SickLeavesConverter();

        public CalendarEventsController(IWorkHoursCredit workHoursCredit, ISickLeaves sickLeaves, IEmployees employees)
        {
            this.workHoursCredit = workHoursCredit;
            this.sickLeaves = sickLeaves;
            this.employees = employees;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventWithIdModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int employeeId, CancellationToken token)
        {
            var workHoursEvents = await this.workHoursCredit.GetCalendarEventsAsync(new EmployeeId(employeeId), token);
            var sickLeaveEvents = await this.sickLeaves.GetCalendarEventsAsync(new EmployeeId(employeeId), token);
            var allEvents = workHoursEvents
                .Select(this.workHoursConverter.ToCalendarEventWithId)
                .Union(sickLeaveEvents.Select(this.sickLeavesConverter.ToCalendarEventWithId));

            return this.Ok(allEvents);
        }


        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(typeof(CalendarEventModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int employeeId, string eventId, CancellationToken token)
        {
            if (Guid.TryParse(eventId, out var guidId))
            {
                var change = await this.workHoursCredit.GetCalendarEventAsync(new EmployeeId(employeeId), guidId, token);
                if (change != null)
                {
                    return this.Ok(this.workHoursConverter.ToCalendarEvent(change));
                }
            }

            if (int.TryParse(eventId, out var intId))
            {
                var sickLeave = await this.sickLeaves.GetCalendarEventAsync(new EmployeeId(employeeId), intId, token);
                if (sickLeave != null)
                {
                    return this.Ok(this.sickLeavesConverter.ToCalendarEvent(sickLeave));
                }
            }

            return this.NotFound();
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CalendarEventWithIdModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(int employeeId, [FromBody] CalendarEventModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            CalendarEventWithIdModel createdModel;

            switch (model.Type)
            {
                case CalendarEventTypes.Dayoff:
                case CalendarEventTypes.Workout:
                    var type = model.Type == CalendarEventTypes.Dayoff ? WorkHoursChangeType.Dayoff : WorkHoursChangeType.Workout;
                    var dayPart = this.workHoursConverter.GetDayPart(model.Dates);
                    var change = await this.workHoursCredit.RequestChangeAsync(new EmployeeId(employeeId), type, model.Dates.StartDate, dayPart);
                    createdModel = this.workHoursConverter.ToCalendarEventWithId(change);
                    break;
                default:
                    return this.BadRequest("Unknown model type");
            }

            return this.CreatedAtAction(nameof(this.Get), new { employeeId, eventId = createdModel.CalendarEventId }, createdModel);
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int employeeId, Guid eventId, [FromBody] CalendarEventModel model)
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
                    return await this.UpdateWorkHoursRequest(new EmployeeId(employeeId), eventId, model, changedBy);
                default:
                    return this.BadRequest("Unsupported event type");
            }
        }

        private async Task<IActionResult> UpdateWorkHoursRequest(EmployeeId employeeId, Guid eventId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            //TODO: possibly add a check for date changes
            if (model.Status != existingEvent.Status.ToString())
            {
                //TODO: add authorization

                if (model.Status == ChangeRequestStatus.Cancelled.ToString())
                {
                    await this.workHoursCredit.CancelRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }

                if (model.Status == ChangeRequestStatus.Rejected.ToString())
                {
                    await this.workHoursCredit.RejectRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }
            }

            return this.NoContent();
        }
    }
}
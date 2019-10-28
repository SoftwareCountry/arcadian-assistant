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

    using SickLeaves.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    //TODO: fuck me, that code has to be split apart...
    [Route("/api/employees/{employeeId}/events/")]
    [Authorize]
    [ApiController]
    public class CalendarEventsController : Controller
    {
        private readonly IWorkHoursCredit workHoursCredit;
        private readonly ISickLeaves sickLeaves;
        private readonly IEmployees employees;
        private readonly IVacations vacations;
        private readonly WorkHoursConverter workHoursConverter = new WorkHoursConverter();
        private readonly SickLeavesConverter sickLeavesConverter = new SickLeavesConverter();
        private readonly VacationsConverter vacationsConverter = new VacationsConverter();

        public CalendarEventsController(IWorkHoursCredit workHoursCredit, ISickLeaves sickLeaves, IEmployees employees, IVacations vacations)
        {
            this.workHoursCredit = workHoursCredit;
            this.sickLeaves = sickLeaves;
            this.employees = employees;
            this.vacations = vacations;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CalendarEventWithIdModel>>> GetAll(int employeeId, CancellationToken token)
        {
            var workHoursEvents = await this.workHoursCredit.GetCalendarEventsAsync(new EmployeeId(employeeId), token);
            var sickLeaveEvents = await this.sickLeaves.GetCalendarEventsAsync(new EmployeeId(employeeId), token);
            var vacationEvents = await this.vacations.GetCalendarEventsAsync(new EmployeeId(employeeId), token);
            var allEvents = workHoursEvents
                .Select(this.workHoursConverter.ToCalendarEventWithId)
                .Union(sickLeaveEvents.Select(this.sickLeavesConverter.ToCalendarEventWithId))
                .Union(vacationEvents.Select(this.vacationsConverter.ToCalendarEventWithId))
                .ToList();

            return allEvents;
        }


        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CalendarEventModel>> Get(int employeeId, string eventId, CancellationToken token)
        {
            if (Guid.TryParse(eventId, out var guidId))
            {
                var change = await this.workHoursCredit.GetCalendarEventAsync(new EmployeeId(employeeId), guidId, token);
                if (change != null)
                {
                    return this.workHoursConverter.ToCalendarEvent(change);
                }
            }

            if (int.TryParse(eventId, out var intId))
            {
                var sickLeave = await this.sickLeaves.GetCalendarEventAsync(new EmployeeId(employeeId), intId, token);
                if (sickLeave != null)
                {
                    return this.sickLeavesConverter.ToCalendarEvent(sickLeave);
                }
            }

            return this.NotFound();
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CalendarEventWithIdModel>> Create(int employeeId, [FromBody] CalendarEventModel model, CancellationToken token)
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
                case CalendarEventTypes.Sickleave:
                    var sickleave = await this.sickLeaves.CreateSickLeaveAsync(new EmployeeId(employeeId), model.Dates.StartDate, model.Dates.EndDate);
                    createdModel = this.sickLeavesConverter.ToCalendarEventWithId(sickleave);
                    break;
                case CalendarEventTypes.Vacation:
                    var vacation = await this.vacations.RequestVacationAsync(new EmployeeId(employeeId), model.Dates.StartDate, model.Dates.EndDate);
                    createdModel = this.vacationsConverter.ToCalendarEventWithId(vacation);
                    break;
                default:
                    return this.BadRequest("Unknown model type");
            }

            return this.CreatedAtAction(nameof(this.Get), new { employeeId, eventId = createdModel.CalendarEventId }, createdModel);
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int employeeId, string eventId, [FromBody] CalendarEventModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var changedBy = (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), CancellationToken.None)).First();

            if (changedBy == null)
            {
                return this.Unauthorized();
            }

            try
            {
                switch (model.Type)
                {
                    case CalendarEventTypes.Dayoff:
                    case CalendarEventTypes.Workout:
                        return await this.UpdateWorkHoursRequest(new EmployeeId(employeeId), eventId, model, changedBy);
                    case CalendarEventTypes.Sickleave:
                        return await this.UpdateSickLeave(new EmployeeId(employeeId), eventId, model, changedBy);
                    case CalendarEventTypes.Vacation:
                        return await this.UpdateVacation(new EmployeeId(employeeId), eventId, model, changedBy);
                    default:
                        return this.BadRequest("Unsupported event type");
                }
            }
            catch (AggregateException ae) when (ae.InnerException is ArgumentException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        private async Task<IActionResult> UpdateWorkHoursRequest(EmployeeId employeeId, string stringEventId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            if (!Guid.TryParse(stringEventId, out var eventId))
            {
                return this.BadRequest("EventId is not guid");
            }

            var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (existingEvent.Status == ChangeRequestStatus.Cancelled)
            {
                return this.BadRequest("Cannot change cancelled event");
            }

            //TODO: possibly add a check for date changes
            if (model.Status != existingEvent.Status.ToString())
            {
                //TODO: add authorization

                if (model.Status == ChangeRequestStatus.Cancelled.ToString())
                {
                    await this.workHoursCredit.CancelRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }
                else if (model.Status == ChangeRequestStatus.Rejected.ToString())
                {
                    await this.workHoursCredit.RejectRequestAsync(employeeId, eventId, null, changedBy.EmployeeId);
                }
                else
                {
                    return this.BadRequest("Unsupported status transition");
                }
            }

            return this.NoContent();
        }

        private async Task<IActionResult> UpdateSickLeave(EmployeeId employeeId, string stringEventId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            if (!int.TryParse(stringEventId, out var eventId))
            {
                return this.BadRequest("EventId is not int");
            }

            var existingEvent = await this.sickLeaves.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (existingEvent.Status == SickLeaveStatus.Cancelled)
            {
                return this.BadRequest("Cannot change cancelled event");
            }

            if (model.Status != existingEvent.Status.ToString())
            {
                if (model.Status == SickLeaveStatus.Cancelled.ToString())
                {
                    await this.sickLeaves.CancelSickLeaveAsync(employeeId, eventId, changedBy.EmployeeId);
                }
                else
                {
                    return this.BadRequest("Unsupported status transition");
                }
            }

            if (model.Dates.EndDate != existingEvent.EndDate)
            {
                await this.sickLeaves.ProlongSickLeaveAsync(employeeId, eventId, model.Dates.EndDate);
            }

            return this.NoContent();
        }

        private async Task<IActionResult> UpdateVacation(EmployeeId employeeId, string stringEventId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            if (!int.TryParse(stringEventId, out var eventId))
            {
                return this.BadRequest("id is not an integer");
            }

            var existingEvent = await this.vacations.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (existingEvent.Status != VacationStatus.Requested)
            {
                return this.BadRequest("Can only change requests in Requested status");
            }

            if (!Enum.TryParse<VacationStatus>(model.Status, out var newStatus))
            {
                return this.BadRequest("Unknown vacation status");
            }

            var currentUser = (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), CancellationToken.None))
                .FirstOrDefault();

            if (currentUser == null)
            {
                return this.Forbid();
            }

            if (newStatus == VacationStatus.Cancelled)
            {
                await this.vacations.CancelVacationAsync(employeeId, eventId, currentUser.EmployeeId, string.Empty);
            }

            if (newStatus == VacationStatus.Requested && (model.Dates.StartDate != existingEvent.StartDate || model.Dates.EndDate != existingEvent.EndDate))
            {
                await this.vacations.ChangeDatesAsync(employeeId, eventId, model.Dates.StartDate, model.Dates.EndDate);
            }

            return this.NoContent();
        }
    }
}
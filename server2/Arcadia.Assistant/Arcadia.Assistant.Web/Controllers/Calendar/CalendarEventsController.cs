namespace Arcadia.Assistant.Web.Controllers.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authorization;
    using Authorization.Requirements;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models.Calendar;

    using Permissions.Contracts;

    using SickLeaves.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    //TODO: fuck me, that code has to be split apart...
    [Route("/api/employees/{employeeId}/events/")]
    [Authorize(Policies.UserIsEmployee)]
    [ApiController]
    public class CalendarEventsController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IEmployees employees;
        private readonly CalendarEventIdConverter idConverter = new CalendarEventIdConverter();
        private readonly ISickLeaves sickLeaves;
        private readonly SickLeavesConverter sickLeavesConverter = new SickLeavesConverter();
        private readonly IVacations vacations;
        private readonly VacationsConverter vacationsConverter = new VacationsConverter();
        private readonly WorkHoursConverter workHoursConverter = new WorkHoursConverter();
        private readonly IWorkHoursCredit workHoursCredit;

        public CalendarEventsController(
            IWorkHoursCredit workHoursCredit, ISickLeaves sickLeaves, IEmployees employees, IVacations vacations,
            IAuthorizationService authorizationService)
        {
            this.workHoursCredit = workHoursCredit;
            this.sickLeaves = sickLeaves;
            this.employees = employees;
            this.vacations = vacations;
            this.authorizationService = authorizationService;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CalendarEventWithIdModel>>> GetAll(
            int employeeId, CancellationToken token)
        {
            var employee = new EmployeeId(employeeId);
            if (!(await this.authorizationService.AuthorizeAsync(this.User, employee, new ReadCalendarEvents()))
                .Succeeded)
            {
                return this.Forbid();
            }

            var workHoursEvents = await this.workHoursCredit.GetCalendarEventsAsync(employee, token);
            var sickLeaveEvents = await this.sickLeaves.GetCalendarEventsAsync(employee, token);
            var vacationEvents = await this.vacations.GetCalendarEventsAsync(employee, token);
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
            if (!(await this.authorizationService.AuthorizeAsync(this.User, employeeId, new ReadCalendarEvents()))
                .Succeeded)
            {
                return this.Forbid();
            }

            if (this.idConverter.TryParseWorkHoursChangeId(eventId, out var changeId))
            {
                var change =
                    await this.workHoursCredit.GetCalendarEventAsync(new EmployeeId(employeeId), changeId, token);
                if (change != null)
                {
                    return this.workHoursConverter.ToCalendarEvent(change);
                }
            }
            else if (this.idConverter.TryParseSickLeaveId(eventId, out var sickLeaveId))
            {
                var sickLeave =
                    await this.sickLeaves.GetCalendarEventAsync(new EmployeeId(employeeId), sickLeaveId, token);
                if (sickLeave != null)
                {
                    return this.sickLeavesConverter.ToCalendarEvent(sickLeave);
                }
            }
            else if (this.idConverter.TryParseVacationId(eventId, out var vacationId))
            {
                var vacation =
                    await this.vacations.GetCalendarEventAsync(new EmployeeId(employeeId), vacationId, token);
                if (vacation != null)
                {
                    return this.vacationsConverter.ToCalendarEvent(vacation);
                }
            }

            return this.NotFound();
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CalendarEventWithIdModel>> Create(
            int employeeId, [FromBody] CalendarEventModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (!(await this.authorizationService.AuthorizeAsync(this.User, new EmployeeId(employeeId),
                new CreateCalendarEvents())).Succeeded)
            {
                return this.Forbid();
            }

            CalendarEventWithIdModel createdModel;

            switch (model.Type)
            {
                case CalendarEventTypes.Dayoff:
                case CalendarEventTypes.Workout:
                    var type = model.Type == CalendarEventTypes.Dayoff
                        ? WorkHoursChangeType.Dayoff
                        : WorkHoursChangeType.Workout;
                    var dayPart = this.workHoursConverter.GetDayPart(model.Dates);
                    var change = await this.workHoursCredit.RequestChangeAsync(new EmployeeId(employeeId), type,
                        model.Dates.StartDate, dayPart);
                    createdModel = this.workHoursConverter.ToCalendarEventWithId(change);
                    break;
                case CalendarEventTypes.Sickleave:
                    var sickleave = await this.sickLeaves.CreateSickLeaveAsync(new EmployeeId(employeeId),
                        model.Dates.StartDate, model.Dates.EndDate, new UserIdentity(this.User.Identity.Name!));
                    createdModel = this.sickLeavesConverter.ToCalendarEventWithId(sickleave);
                    break;
                case CalendarEventTypes.Vacation:
                    var vacation = await this.vacations.RequestVacationAsync(new EmployeeId(employeeId),
                        model.Dates.StartDate, model.Dates.EndDate);
                    createdModel = this.vacationsConverter.ToCalendarEventWithId(vacation);
                    break;
                default:
                    return this.BadRequest("Unknown model type");
            }

            return this.CreatedAtAction(nameof(this.Get), new { employeeId, eventId = createdModel.CalendarEventId },
                createdModel);
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
            var changedBy =
                (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity),
                    CancellationToken.None)).First();

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
                        return await this.UpdateSickLeave(new EmployeeId(employeeId), eventId, model,
                            new UserIdentity(this.User.Identity.Name!));
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

        private async Task<IActionResult> UpdateWorkHoursRequest(
            EmployeeId employeeId, string dtoId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            if (!this.idConverter.TryParseWorkHoursChangeId(dtoId, out var eventId))
            {
                return this.BadRequest("EventId is not guid");
            }

            var existingEvent =
                await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
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

        private async Task<IActionResult> UpdateSickLeave(
            EmployeeId employeeId, string dtoId, CalendarEventModel model, UserIdentity changedBy)
        {
            if (!this.idConverter.TryParseSickLeaveId(dtoId, out var eventId))
            {
                return this.BadRequest("EventId is not int");
            }

            var existingEvent =
                await this.sickLeaves.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (model.Status != existingEvent.Status.ToString() && model.Status == SickLeaveStatus.Cancelled.ToString())
            {
                await this.sickLeaves.CancelSickLeaveAsync(employeeId, eventId, changedBy);
            }

            if (model.Dates.EndDate != existingEvent.EndDate)
            {
                await this.sickLeaves.ProlongSickLeaveAsync(employeeId, eventId, model.Dates.EndDate, changedBy);
            }

            return this.NoContent();
        }

        private async Task<IActionResult> UpdateVacation(
            EmployeeId employeeId, string dtoId, CalendarEventModel model, EmployeeMetadata changedBy)
        {
            if (!this.idConverter.TryParseVacationId(dtoId, out var eventId))
            {
                return this.BadRequest("id is not an integer");
            }

            var existingEvent = await this.vacations.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (!Enum.TryParse<VacationStatus>(model.Status, out var newStatus))
            {
                return this.BadRequest("Unknown vacation status");
            }

            var currentUser =
                (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity),
                    CancellationToken.None))
                .FirstOrDefault();

            if (currentUser == null)
            {
                return this.Forbid();
            }

            if (existingEvent.Status != newStatus)
            {
                if (newStatus == VacationStatus.Requested)
                {
                    return this.BadRequest("Cannot move from any status to Requested status");
                }

                /*
                if (existingEvent.Status == VacationStatus.Approved && (newStatus == VacationStatus.Cancelled || newStatus == )
                {
                    await this.vacations.CancelVacationAsync(employeeId, eventId, currentUser.EmployeeId, string.Empty);
                }*/

                if (newStatus == VacationStatus.Cancelled)
                {
                    await this.vacations.CancelVacationAsync(employeeId, eventId, employeeId, "");
                }

                if (newStatus == VacationStatus.Rejected)
                {
                    await this.vacations.RejectVacationAsync(employeeId, eventId, employeeId);
                }
            }
            else
            {
                if (newStatus == VacationStatus.Requested && (model.Dates.StartDate != existingEvent.StartDate ||
                    model.Dates.EndDate != existingEvent.EndDate))
                {
                    await this.vacations.ChangeDatesAsync(employeeId, eventId, model.Dates.StartDate,
                        model.Dates.EndDate);
                }
            }

            return this.NoContent();
        }
    }
}
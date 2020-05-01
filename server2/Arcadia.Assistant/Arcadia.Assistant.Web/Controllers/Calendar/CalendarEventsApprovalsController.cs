namespace Arcadia.Assistant.Web.Controllers.Calendar
{
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

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/{eventId}/approvals")]
    [Authorize(Policies.UserIsEmployee)]
    [ApiController]
    public class CalendarEventsApprovalsController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IEmployees employees;

        private readonly CalendarEventIdConverter idConverter = new CalendarEventIdConverter();
        private readonly IVacations vacations;
        private readonly IWorkHoursCredit workHoursCredit;

        public CalendarEventsApprovalsController(
            IWorkHoursCredit workHoursCredit, IVacations vacations, IEmployees employees,
            IAuthorizationService authorizationService)
        {
            this.workHoursCredit = workHoursCredit;
            this.vacations = vacations;
            this.employees = employees;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CalendarEventApprovalModel[]>> GetEventApprovals(
            int employeeId, string eventId, CancellationToken token)
        {
            if (!(await this.authorizationService.AuthorizeAsync(this.User, new EmployeeId(employeeId),
                new ReadCalendarEvents())).Succeeded)
            {
                return this.Forbid();
            }

            if (this.idConverter.TryParseWorkHoursChangeId(eventId, out var changeId))
            {
                var approvals =
                    await this.workHoursCredit.GetApprovalsAsync(new EmployeeId(employeeId), changeId, token);
                if (approvals == null)
                {
                    return this.NotFound();
                }

                return approvals.Select(x => new CalendarEventApprovalModel(x.ApproverId.ToString(), x.Timestamp))
                    .ToArray();
            }

            if (this.idConverter.TryParseVacationId(eventId, out var vacationId))
            {
                var vacation =
                    await this.vacations.GetCalendarEventAsync(new EmployeeId(employeeId), vacationId, token);
                if (vacation == null)
                {
                    return this.NotFound();
                }

                return vacation.Approvals
                    .Select(x => new CalendarEventApprovalModel(x.ApproverId.ToString(), x.Timestamp)).ToArray();
            }

            return this.NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveEvent(int employeeId, string eventId)
        {
            if (!(await this.authorizationService.AuthorizeAsync(this.User, new EmployeeId(employeeId),
                new ApprovePendingCalendarEvent())).Succeeded)
            {
                return this.Forbid();
            }

            var approver =
                (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity),
                    CancellationToken.None)).FirstOrDefault();
            if (approver == null)
            {
                return this.Forbid();
            }

            if (this.idConverter.TryParseWorkHoursChangeId(eventId, out var changeId))
            {
                var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(new EmployeeId(employeeId),
                    changeId, CancellationToken.None);
                if (existingEvent == null)
                {
                    return this.NotFound();
                }

                await this.workHoursCredit.ApproveRequestAsync(new EmployeeId(employeeId), changeId,
                    approver.EmployeeId);
            }
            else if (this.idConverter.TryParseVacationId(eventId, out var vacationId))
            {
                var existingEvent = await this.vacations.GetCalendarEventAsync(new EmployeeId(employeeId), vacationId,
                    CancellationToken.None);
                if (existingEvent == null)
                {
                    return this.NotFound();
                }

                await this.vacations.ApproveVacationAsync(new EmployeeId(employeeId), vacationId, approver.EmployeeId);
            }
            else
            {
                return this.NotFound();
            }

            return this.Ok();
        }
    }
}
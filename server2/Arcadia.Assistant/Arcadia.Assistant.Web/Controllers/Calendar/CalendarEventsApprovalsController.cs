namespace Arcadia.Assistant.Web.Controllers.Calendar
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models.Calendar;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/{eventId}/approvals")]
    [ApiController]
    public class CalendarEventsApprovalsController : Controller
    {
        private readonly IWorkHoursCredit workHoursCredit;

        public CalendarEventsApprovalsController(IWorkHoursCredit workHoursCredit)
        {
            this.workHoursCredit = workHoursCredit;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CalendarEventApprovalWithTimestampModel[]>> GetEventApprovals(int employeeId, string eventId, CancellationToken token)
        {
            var approvals = await this.workHoursCredit.GetApprovalsAsync(new EmployeeId(employeeId), Guid.Parse(eventId), token);
            if (approvals == null)
            {
                return this.NotFound();
            }

            return approvals.Select(x => new CalendarEventApprovalWithTimestampModel(x.Timestamp, x.ApproverId.ToString())).ToArray();
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveEvent(int employeeId, Guid eventId, [FromBody] CalendarEventApprovalModel model)
        {
            var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(new EmployeeId(employeeId), eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            if (!int.TryParse(model.ApproverId, out var approverId))
            {
                return this.BadRequest("ApproverId must convert to int");
            }

            await this.workHoursCredit.ApproveRequestAsync(new EmployeeId(employeeId), eventId, new EmployeeId(approverId));
            return this.Ok();
        }
    }
}
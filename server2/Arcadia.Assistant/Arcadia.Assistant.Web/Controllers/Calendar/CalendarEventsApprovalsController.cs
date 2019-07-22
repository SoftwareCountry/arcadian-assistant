using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Http;

    using Models.Calendar;

    using WorkHoursCredit.Contracts;

    [Route("/api/employees/{employeeId}/events/{eventId}/approvals")]
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
        [ProducesResponseType(typeof(IEnumerable<CalendarEventApprovalWithTimestampModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventApprovals(int employeeId, string eventId, CancellationToken token)
        {
            var approvals = await this.workHoursCredit.GetApprovalsAsync(new EmployeeId(employeeId), Guid.Parse(eventId), token);
            if (approvals == null)
            {
                return this.NotFound();
            }

            return this.Ok(approvals.Select(x => new CalendarEventApprovalWithTimestampModel(x.Timestamp, x.ApproverId.ToString())));
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
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
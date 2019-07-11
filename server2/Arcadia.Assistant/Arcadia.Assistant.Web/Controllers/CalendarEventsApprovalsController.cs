using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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
        public async Task<IActionResult> GetEventApprovals(string employeeId, string eventId, CancellationToken token)
        {
            var approvals = await this.workHoursCredit.GetApprovalsAsync(employeeId, Guid.Parse(eventId), token);
            if (approvals == null)
            {
                return this.NotFound();
            }

            return this.Ok(approvals.Select(x => new CalendarEventApprovalWithTimestampModel(x.Timestamp, x.ApproverId)));
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ApproveEvent(string employeeId, Guid eventId, [FromBody] CalendarEventApprovalModel model, CancellationToken token)
        {
            var existingEvent = await this.workHoursCredit.GetCalendarEventAsync(employeeId, eventId, CancellationToken.None);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            await this.workHoursCredit.ApproveRequestAsync(employeeId, eventId, model.ApproverId);
            return this.Ok();
        }
    }
}
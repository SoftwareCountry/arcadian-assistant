namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models.Calendar;

    [Route("/api/employees/{employeeId}/events/{eventId}/approvals")]
    [Authorize(Policies.UserIsEmployee)]
    public class CalendarEventsApprovalsController : Controller
    {
        private readonly ITimeoutSettings timeoutSettings;
        private readonly IEmployeesRegistry employeesRegistry;
        private readonly IAuthorizationService authorizationService;

        public CalendarEventsApprovalsController(
            ITimeoutSettings timeoutSettings,
            IEmployeesRegistry employeesRegistry,
            IAuthorizationService authorizationService)
        {
            this.timeoutSettings = timeoutSettings;
            this.employeesRegistry = employeesRegistry;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventApprovalModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventApprovals(string employeeId, string eventId, CancellationToken token)
        {
            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);
            if (employee == null)
            {
                return this.NotFound();
            }

            var authorizationResult = await this.authorizationService.AuthorizeAsync(this.User, employee, new ReadCalendarEvents());
            if (!authorizationResult.Succeeded)
            {
                return this.Forbid();
            }

            var requestedEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar.CalendarActor, eventId, token);
            if (requestedEvent == null)
            {
                return this.NotFound();
            }

            var response = await employee.Calendar.CalendarActor.Ask<GetCalendarEventApprovals.Response>(
                new GetCalendarEventApprovals(requestedEvent),
                this.timeoutSettings.Timeout,
                token);

            switch (response)
            {
                case GetCalendarEventApprovals.SuccessResponse result:
                    var model = result.Approvals.Select(a => new CalendarEventApprovalModel(a));
                    return this.Ok(model);

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ApproveEvent(string employeeId, string eventId, [FromBody]CalendarEventApprovalModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);
            if (employee == null)
            {
                return this.NotFound();
            }

            var approver = await this.GetEmployeeOrDefaultAsync(model.ApproverId, token);
            if (approver == null)
            {
                return this.BadRequest(this.ModelState);
            }

            var authorizationResult = await this.authorizationService.AuthorizeAsync(this.User, approver, new EditPendingCalendarEvents());
            if (!authorizationResult.Succeeded)
            {
                return this.Forbid();
            }

            var requestedEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar.CalendarActor, eventId, token);
            if (requestedEvent == null)
            {
                return this.NotFound();
            }

            if (!requestedEvent.IsPending)
            {
                return this.Conflict();
            }

            var response = await employee.Calendar.CalendarActor.Ask<ApproveCalendarEvent.Response>(
                new ApproveCalendarEvent(requestedEvent, approver.Metadata.EmployeeId),
                this.timeoutSettings.Timeout,
                token);

            switch (response)
            {
                case ApproveCalendarEvent.SuccessResponse _:
                    return this.Accepted();

                case ApproveCalendarEvent.BadRequestResponse err:
                    return this.BadRequest(err.Message);

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<EmployeeContainer> GetEmployeeOrDefaultAsync(string employeeId, CancellationToken token)
        {
            var query = new EmployeesQuery().WithId(employeeId);
            var employees = await this.employeesRegistry.SearchAsync(query, token);

            return employees.SingleOrDefault();
        }

        private async Task<CalendarEvent> GetCalendarEventOrDefaultAsync(IActorRef calendarActor, string eventId, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;
            var calendarEvent = await calendarActor.Ask<GetCalendarEvent.Response>(new GetCalendarEvent(eventId), timeout, token);
            switch (calendarEvent)
            {
                case GetCalendarEvent.Response.Found response:
                    return response.Event;

                case GetCalendarEvent.Response.NotFound _:
                    return null;

                default:
                    return null;
            }
        }
    }
}
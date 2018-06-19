namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Organization.Abstractions;
    using Requirements;
    using Security;
    using System.Threading.Tasks;
    using Models.Calendar;

    public class EditCalendarEventsPermissionHandler : AuthorizationHandler<EditCalendarEvents, EmployeeContainer>
    {
        private readonly IPermissionsLoader permissionsLoader;

        public EditCalendarEventsPermissionHandler(IPermissionsLoader permissionsLoader)
        {
            this.permissionsLoader = permissionsLoader;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EditCalendarEvents requirement, EmployeeContainer resource)
        {
            var allPermissions = await this.permissionsLoader.LoadAsync(context.User);
            var employeePermissions = allPermissions.GetPermissions(resource);

            var existingEvent = requirement.ExistingEvent;
            var updatedEvent = requirement.UpdatedEvent;
            var statusChanged = updatedEvent.Status != existingEvent.Status;

            var hasPermissions = true;

            hasPermissions &= this.CheckIfApproval(statusChanged, existingEvent, updatedEvent, employeePermissions);
            hasPermissions &= this.CheckIfRejected(statusChanged, existingEvent, updatedEvent, employeePermissions);

            if (updatedEvent.Type == CalendarEventTypes.Sickleave)
            {
                hasPermissions &= this.CheckSickLeave(existingEvent, updatedEvent, employeePermissions);
            }

            if (hasPermissions)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private bool CheckIfApproval(bool statusChanged, CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(existingEvent.Type);

            if (statusChanged && updatedEvent.Status == approved)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.ApproveCalendarEvents);
            }

            return true;
        }

        private bool CheckIfRejected(bool statusChanged, CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var rejected = calendarEventStatuses.RejectedForType(existingEvent.Type);

            if (statusChanged && updatedEvent.Status == rejected)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.RejectCalendarEvents);
            }

            return true;
        }

        private bool CheckSickLeave(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            if (updatedEvent.Status == existingEvent.Status
                && updatedEvent.Status == SickLeaveStatuses.Approved
                && updatedEvent.Dates != existingEvent.Dates)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.ProlongSickLeave);
            }

            if (updatedEvent.Status != existingEvent.Status && updatedEvent.Status == SickLeaveStatuses.Completed)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.CompleteSickLeave);
            }

            return true;
        }
    }
}

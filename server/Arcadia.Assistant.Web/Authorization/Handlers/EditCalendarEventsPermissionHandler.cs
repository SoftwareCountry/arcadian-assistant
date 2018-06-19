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

            var hasPermissions = true;

            hasPermissions &= this.CheckIfApproval(existingEvent, updatedEvent, employeePermissions);
            hasPermissions &= this.CheckIfRejected(existingEvent, updatedEvent, employeePermissions);

            if (updatedEvent.Type == CalendarEventTypes.Sickleave)
            {
                hasPermissions &= this.CheckSickLeave(existingEvent, updatedEvent, employeePermissions);
            }

            if (hasPermissions)
            {
                context.Succeed(requirement);
            }
        }

        private bool CheckIfApproval(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(existingEvent.Type);
            var statusChanged = this.StatusChanged(existingEvent, updatedEvent);

            if (statusChanged && updatedEvent.Status == approved)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.ApproveCalendarEvents);
            }

            return true;
        }

        private bool CheckIfRejected(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var rejected = calendarEventStatuses.RejectedForType(existingEvent.Type);
            var statusChanged = this.StatusChanged(existingEvent, updatedEvent);

            if (statusChanged && updatedEvent.Status == rejected)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.RejectCalendarEvents);
            }

            return true;
        }

        private bool CheckSickLeave(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var statusChanged = this.StatusChanged(existingEvent, updatedEvent);

            if (!statusChanged
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
        
        private bool StatusChanged(CalendarEvent existingEvent, CalendarEventsModel updatedEvent)
        {
            return existingEvent.Status != updatedEvent.Status;
        }
    }
}

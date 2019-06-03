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

            hasPermissions &= CheckIfApproval(existingEvent, updatedEvent, employeePermissions);
            hasPermissions &= CheckIfRejected(existingEvent, updatedEvent, employeePermissions);
            hasPermissions &= CheckIfCancelled(existingEvent, updatedEvent, employeePermissions);

            if (hasPermissions)
            {
                context.Succeed(requirement);
            }
        }

        private static bool CheckIfApproval(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(existingEvent.Type);
            var statusChanged = StatusChanged(existingEvent, updatedEvent);

            if (statusChanged && updatedEvent.Status == approved)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.ApproveCalendarEvents);
            }

            return true;
        }

        private static bool CheckIfRejected(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var rejected = calendarEventStatuses.RejectedForType(existingEvent.Type);
            var statusChanged = StatusChanged(existingEvent, updatedEvent);

            if (statusChanged && updatedEvent.Status == rejected)
            {
                return employeePermissions.HasFlag(EmployeePermissionsEntry.RejectCalendarEvents);
            }

            return true;
        }

        private static bool CheckIfCancelled(CalendarEvent existingEvent, CalendarEventsModel updatedEvent, EmployeePermissionsEntry employeePermissions)
        {
            var calendarEventStatuses = new CalendarEventStatuses();
            var approvedStatus = calendarEventStatuses.ApprovedForType(existingEvent.Type);
            var cancelledStatus = calendarEventStatuses.CancelledForType(updatedEvent.Type);
            var statusChanged = StatusChanged(existingEvent, updatedEvent);

            if (statusChanged && existingEvent.Status == approvedStatus && updatedEvent.Status == cancelledStatus)
            {
                var permissionExists = employeePermissions.HasFlag(EmployeePermissionsEntry.CancelApprovedCalendarEvents);

                if (!permissionExists && existingEvent.Type == CalendarEventTypes.Sickleave)
                {
                    permissionExists = employeePermissions.HasFlag(EmployeePermissionsEntry.CancelApprovedSickLeaves);
                }

                return permissionExists;
            }

            return true;
        }

        private static bool StatusChanged(CalendarEvent existingEvent, CalendarEventsModel updatedEvent)
        {
            return existingEvent.Status != updatedEvent.Status;
        }
    }
}

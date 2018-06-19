namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Organization.Abstractions;
    using Requirements;
    using Security;
    using System.Threading.Tasks;

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

            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(existingEvent.Type);
            var rejected = calendarEventStatuses.RejectedForType(existingEvent.Type);

            if (updatedEvent.Status != existingEvent.Status && updatedEvent.Status == approved)
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.ApproveCalendarEvents))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            if (updatedEvent.Status != existingEvent.Status && updatedEvent.Status == rejected)
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.RejectCalendarEvents))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            if (updatedEvent.Type == CalendarEventTypes.Sickleave)
            {
                if (updatedEvent.Status == existingEvent.Status
                    && updatedEvent.Status == SickLeaveStatuses.Approved
                    && updatedEvent.Dates != existingEvent.Dates)
                {
                    if (employeePermissions.HasFlag(EmployeePermissionsEntry.ProlongSickLeave))
                    {
                        context.Succeed(requirement);
                    }

                    return;
                }

                if (updatedEvent.Status != existingEvent.Status && updatedEvent.Status == SickLeaveStatuses.Completed)
                {
                    if (employeePermissions.HasFlag(EmployeePermissionsEntry.CompleteSickLeave))
                    {
                        context.Succeed(requirement);
                    }

                    return;
                }
            }

            context.Succeed(requirement);
        }
    }
}

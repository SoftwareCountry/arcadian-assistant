namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading.Tasks;
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Organization.Abstractions;
    using Requirements;
    using Security;

    public class EditCommonCalendarEventsStatusesPermissionHandler : AuthorizationHandler<EditCommonCalendarEventsStatuses, EmployeeContainer>
    {
        private readonly IPermissionsLoader permissionsLoader;

        public EditCommonCalendarEventsStatusesPermissionHandler(IPermissionsLoader permissionsLoader)
        {
            this.permissionsLoader = permissionsLoader;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EditCommonCalendarEventsStatuses requirement, EmployeeContainer resource)
        {
            var allPermissions = await this.permissionsLoader.LoadAsync(context.User);
            var employeePermissions = allPermissions.GetPermissions(resource);

            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(requirement.ExistingEvent.Type);
            var rejected = calendarEventStatuses.RejectedForType(requirement.ExistingEvent.Type);

            var updatedEvent = requirement.UpdatedEvent;
            var existingEvent = requirement.ExistingEvent;

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

            context.Succeed(requirement);
        }
    }
}
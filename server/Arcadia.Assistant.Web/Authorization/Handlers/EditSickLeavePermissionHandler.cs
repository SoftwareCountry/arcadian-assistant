using System.Threading.Tasks;

namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Organization.Abstractions;
    using Requirements;
    using Security;

    public class EditSickLeavePermissionHandler : AuthorizationHandler<EditSickLeave, EmployeeContainer>
    {
        private readonly IPermissionsLoader permissionsLoader;

        public EditSickLeavePermissionHandler(IPermissionsLoader permissionsLoader)
        {
            this.permissionsLoader = permissionsLoader;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EditSickLeave requirement, EmployeeContainer resource)
        {
            var existingEvent = requirement.ExistingEvent;
            var updatedEvent = requirement.UpdatedEvent;

            if (updatedEvent.Type != CalendarEventTypes.Sickleave)
            {
                context.Succeed(requirement);
                return;
            }

            var allPermissions = await this.permissionsLoader.LoadAsync(context.User);
            var employeePermissions = allPermissions.GetPermissions(resource);

            if (updatedEvent.Status == existingEvent.Status
                && updatedEvent.Status == SickLeaveStatuses.Approved
                && (updatedEvent.Dates.StartDate != existingEvent.Dates.StartDate || updatedEvent.Dates.EndDate != existingEvent.Dates.EndDate))
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

            context.Succeed(requirement);
        }
    }
}

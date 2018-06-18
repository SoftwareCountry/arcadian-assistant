namespace Arcadia.Assistant.Web.Authorization.Handlers
{
    using System.Threading.Tasks;
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Organization.Abstractions;
    using Requirements;
    using Security;

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

            var calendarEventStatuses = new CalendarEventStatuses();
            var approved = calendarEventStatuses.ApprovedForType(requirement.ExistingEvent.Type);
            var rejected = calendarEventStatuses.RejectedForType(requirement.ExistingEvent.Type);
            var completed = calendarEventStatuses.CompletedForType(requirement.ExistingEvent.Type);

            if (requirement.UpdatedEvent.Status == approved 
                && (requirement.UpdatedEvent.Dates.StartDate != requirement.ExistingEvent.Dates.StartDate 
                    || requirement.UpdatedEvent.Dates.EndDate != requirement.ExistingEvent.Dates.EndDate))
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.ProlongCalendarEvents))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return;
            }

            if (requirement.UpdatedEvent.Status == approved)
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.ApproveCalendarEvents))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return;
            }

            if (requirement.UpdatedEvent.Status == rejected)
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.RejectCalendarEvents))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return;
            }

            if (requirement.UpdatedEvent.Status == completed)
            {
                if (employeePermissions.HasFlag(EmployeePermissionsEntry.CompleteCalendarEvents))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return;
            }
        }
    }
}
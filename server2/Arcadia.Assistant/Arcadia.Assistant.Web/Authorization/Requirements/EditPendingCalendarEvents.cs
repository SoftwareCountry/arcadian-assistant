namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class EditPendingCalendarEvents : RequiredEmployeePermissions
    {
        public EditPendingCalendarEvents()
            : base(EmployeePermissionsEntry.EditPendingCalendarEvents)
        {
        }
    }
}
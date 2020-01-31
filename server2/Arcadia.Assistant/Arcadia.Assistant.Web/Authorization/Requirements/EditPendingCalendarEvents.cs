namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class EditPendingCalendarEvents : EmployeePermissionsRequirement
    {
        public EditPendingCalendarEvents()
            : base(EmployeePermissionsEntry.EditPendingCalendarEvents)
        {
        }
    }
}
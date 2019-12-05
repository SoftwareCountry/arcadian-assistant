namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class CreateCalendarEvents : RequiredEmployeePermissions
    {
        public CreateCalendarEvents()
            : base(EmployeePermissionsEntry.CreateCalendarEvents)
        {
        }
    }
}
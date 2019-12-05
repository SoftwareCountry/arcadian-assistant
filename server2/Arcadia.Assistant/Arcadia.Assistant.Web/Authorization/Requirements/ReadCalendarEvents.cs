namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class ReadCalendarEvents : RequiredEmployeePermissions
    {
        public ReadCalendarEvents()
            :base(EmployeePermissionsEntry.ReadEmployeeCalendarEvents)
        {
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;

    public class ReadCalendarEvents : RequiredEmployeePermissions
    {
        public ReadCalendarEvents()
            :base(EmployeePermissionsEntry.ReadEmployeeCalendarEvents)
        {
        }
    }
}
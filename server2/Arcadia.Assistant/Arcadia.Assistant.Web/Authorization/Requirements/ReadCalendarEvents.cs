namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class ReadCalendarEvents : EmployeePermissionsRequirement
    {
        public ReadCalendarEvents()
            : base(EmployeePermissionsEntry.ReadEmployeeCalendarEvents)
        {
        }
    }
}
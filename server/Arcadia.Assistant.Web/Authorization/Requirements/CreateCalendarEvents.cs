namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;

    public class CreateCalendarEvents : RequiredEmployeePermissions
    {
        public CreateCalendarEvents()
            : base(EmployeePermissionsEntry.CreateCalendarEvents)
        {
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class CreateCalendarEvents : EmployeePermissionsRequirement
    {
        public CreateCalendarEvents()
            : base(EmployeePermissionsEntry.CreateCalendarEvents)
        {
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;

    public class ApproveCalendarEvents : RequiredEmployeePermissions
    {
        public ApproveCalendarEvents()
            : base(EmployeePermissionsEntry.ApproveEmployeeCalendarEvents)
        {
        }
    }
}
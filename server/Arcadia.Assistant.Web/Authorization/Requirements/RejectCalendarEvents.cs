namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Security;

    public class RejectCalendarEvents : RequiredEmployeePermissions
    {
        public RejectCalendarEvents() 
            : base(EmployeePermissionsEntry.RejectEmployeeCalendarEvents)
        {
        }
    }
}

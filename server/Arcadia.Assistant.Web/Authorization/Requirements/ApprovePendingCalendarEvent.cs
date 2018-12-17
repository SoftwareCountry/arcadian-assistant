namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;

    public class ApprovePendingCalendarEvent : RequiredEmployeePermissions
    {
        public ApprovePendingCalendarEvent()
            : base(EmployeePermissionsEntry.ApproveCalendarEvents)
        {
        }
    }
}
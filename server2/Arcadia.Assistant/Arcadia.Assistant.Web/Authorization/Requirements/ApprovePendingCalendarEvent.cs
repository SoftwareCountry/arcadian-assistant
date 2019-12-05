namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class ApprovePendingCalendarEvent : RequiredEmployeePermissions
    {
        public ApprovePendingCalendarEvent()
            : base(EmployeePermissionsEntry.ApproveCalendarEvents)
        {
        }
    }
}
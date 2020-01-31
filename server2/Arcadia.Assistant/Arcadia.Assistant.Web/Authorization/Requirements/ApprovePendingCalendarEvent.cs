namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Permissions.Contracts;

    public class ApprovePendingCalendarEvent : EmployeePermissionsRequirement
    {
        public ApprovePendingCalendarEvent()
            : base(EmployeePermissionsEntry.ApproveCalendarEvents)
        {
        }
    }
}
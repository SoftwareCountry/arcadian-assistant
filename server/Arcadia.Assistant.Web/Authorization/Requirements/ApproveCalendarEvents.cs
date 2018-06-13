namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;
    using Calendar.Abstractions;
    using Models.Calendar;

    public class ApproveCalendarEvents : RequiredEmployeePermissions
    {
        private readonly CalendarEventsModel model;

        public ApproveCalendarEvents(CalendarEventsModel model)
            : base(EmployeePermissionsEntry.ApproveEmployeeCalendarEvents)
        {
            this.model = model;
        }

        public override bool HasPermissions(EmployeePermissionsEntry employeePermissionsEntry)
        {
            var approvedStatus = new CalendarEventStatuses().ApprovedForType(model.Type);

            if (model.Type == approvedStatus)
            {
                return base.HasPermissions(employeePermissionsEntry);
            }

            return true;
        }
    }
}
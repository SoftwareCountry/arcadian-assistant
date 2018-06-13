namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Calendar.Abstractions;
    using Models.Calendar;
    using Security;

    public class RejectCalendarEvents : RequiredEmployeePermissions
    {
        private readonly CalendarEventsModel model;

        public RejectCalendarEvents(CalendarEventsModel model) 
            : base(EmployeePermissionsEntry.RejectEmployeeCalendarEvents)
        {
            this.model = model;
        }

        public override bool HasPermissions(EmployeePermissionsEntry employeePermissionsEntry)
        {
            var rejectedStatus = new CalendarEventStatuses().RejectedForType(model.Type);

            if (model.Type == rejectedStatus)
            {
                return base.HasPermissions(employeePermissionsEntry);
            }

            return true;
        }
    }
}

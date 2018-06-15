namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;
    using Calendar.Abstractions;
    using Models.Calendar;

    public class EditCalendarEvents : RequiredEmployeePermissions
    {
        public EditCalendarEvents()
            : base(EmployeePermissionsEntry.EditCalendarEvents)
        {
        }
    }
}
namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Models.Calendar;
    using Security;

    public class EditPendingCalendarEvents : RequiredEmployeePermissions
    {
        public EditPendingCalendarEvents() 
            : base(EmployeePermissionsEntry.EditPendingCalendarEvents)
        {
        }
    }
}

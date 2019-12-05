namespace Arcadia.Assistant.Sharepoint
{
    using System.Collections.Generic;

    public interface ISharepointDepartmentsCalendarsSettings
    {
        IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; }
    }
}
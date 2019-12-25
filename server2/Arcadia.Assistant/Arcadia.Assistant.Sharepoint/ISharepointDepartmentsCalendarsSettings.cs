namespace Arcadia.Assistant.Sharepoint
{
    using System.Collections.Generic;

    using Models;

    public interface ISharepointDepartmentsCalendarsSettings
    {
        IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; }
    }
}
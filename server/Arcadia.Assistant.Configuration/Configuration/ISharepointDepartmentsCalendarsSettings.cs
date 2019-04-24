namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;

    public interface ISharepointDepartmentsCalendarsSettings
    {
        IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; }
    }
}
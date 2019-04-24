namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SharepointSettings : ISharepointDepartmentsCalendarsSettings
    {
        [Required]
        public string ServerUrl { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        public string CalendarEventIdField { get; set; }

        [Required]
        public IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; set; }
    }
}
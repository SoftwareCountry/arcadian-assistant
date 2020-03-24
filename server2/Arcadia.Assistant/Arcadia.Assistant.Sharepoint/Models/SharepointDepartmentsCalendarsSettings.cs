namespace Arcadia.Assistant.Sharepoint.Models
{
    using System.Collections.Generic;
    using System.Fabric.Description;
    using System.Text.Json;

    public class SharepointDepartmentsCalendarsSettings : ISharepointDepartmentsCalendarsSettings
    {
        public SharepointDepartmentsCalendarsSettings(ConfigurationSection configurationSection)
        {
            var val = configurationSection.Parameters["Value"].Value;
            this.DepartmentsCalendars =
                JsonSerializer.Deserialize<IEnumerable<SharepointDepartmentCalendarMapping>>(val);
        }

        public IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; set; }
    }
}
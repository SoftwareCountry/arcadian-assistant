namespace Arcadia.Assistant.Sharepoint.Models
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Description;
    using System.Text.Json;

    public class SharepointDepartmentsCalendarsSettings : ISharepointDepartmentsCalendarsSettings
    {
        public SharepointDepartmentsCalendarsSettings(ConfigurationSection configurationSection)
        {
            try
            {
                var val = configurationSection.Parameters["Value"].Value;
                this.DepartmentsCalendars = JsonSerializer.Deserialize<IEnumerable<SharepointDepartmentCalendarMapping>>(val);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<SharepointDepartmentCalendarMapping> DepartmentsCalendars { get; set; }
    }
}
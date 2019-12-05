using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Arcadia.Assistant.Sharepoint
{
    using System.Fabric.Description;
    using System.Text.Json.Serialization;

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

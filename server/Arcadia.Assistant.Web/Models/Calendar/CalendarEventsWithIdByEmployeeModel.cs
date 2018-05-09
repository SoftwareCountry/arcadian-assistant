namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;

    public class CalendarEventsWithIdByEmployeeModel
    {
        public Dictionary<string, IEnumerable<CalendarEventsWithIdModel>> Events { get; }

        public CalendarEventsWithIdByEmployeeModel(Dictionary<string, IEnumerable<CalendarEventsWithIdModel>> events)
        {
            this.Events = events;
        }
    }
}
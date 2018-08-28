namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventsWithIdByEmployeeModel
    {
        [DataMember]
        public Dictionary<string, IEnumerable<CalendarEventsWithIdModel>> Events { get; }

        public CalendarEventsWithIdByEmployeeModel(Dictionary<string, IEnumerable<CalendarEventsWithIdModel>> events)
        {
            this.Events = events;
        }
    }
}
namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventsWithIdByEmployeeModel
    {
        public CalendarEventsWithIdByEmployeeModel(Dictionary<string, IEnumerable<CalendarEventWithIdModel>> events)
        {
            this.Events = events;
        }

        [DataMember]
        public Dictionary<string, IEnumerable<CalendarEventWithIdModel>> Events { get; }
    }
}
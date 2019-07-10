namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventsWithIdByEmployeeModel
    {
        [DataMember]
        public Dictionary<string, IEnumerable<CalendarEventWithIdModel>> Events { get; }

        public CalendarEventsWithIdByEmployeeModel(Dictionary<string, IEnumerable<CalendarEventWithIdModel>> events)
        {
            this.Events = events;
        }
    }
}
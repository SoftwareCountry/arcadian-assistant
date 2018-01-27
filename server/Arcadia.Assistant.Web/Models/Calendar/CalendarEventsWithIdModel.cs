namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsWithIdModel : CalendarEventsModel
    {
        [Required]
        public string CalendarEventId { get; set; }

        public CalendarEventsWithIdModel()
        {
        }

        public CalendarEventsWithIdModel(string calendarEventId, DateTime from, DateTime to)
            : base(from, to)
        {
            this.CalendarEventId = calendarEventId;
        }
    }
}
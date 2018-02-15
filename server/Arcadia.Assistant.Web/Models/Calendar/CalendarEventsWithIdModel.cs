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

        public CalendarEventsWithIdModel(string calendarEventId, CalendarEventType type, DatesPeriodModel dates, CalendarEventStatus status)
            : base(type, dates, status)
        {
            this.CalendarEventId = calendarEventId;
        }
    }
}
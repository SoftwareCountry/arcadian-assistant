namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsWithIdModel : CalendarEventsModel
    {
        [Required]
        public string CalendarEventId { get; set; }

        // ReSharper disable once UnusedMember.Global
        public CalendarEventsWithIdModel()
        {
        }

        public CalendarEventsWithIdModel(string calendarEventId, string type, DatesPeriodModel dates, string status)
            : base(type, dates, status)
        {
            this.CalendarEventId = calendarEventId;
        }
    }
}
namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventWithIdModel : CalendarEventModel
    {
        [Required]
        public string CalendarEventId { get; set; }

        // ReSharper disable once UnusedMember.Global
        public CalendarEventWithIdModel()
        {
        }

        public CalendarEventWithIdModel(string calendarEventId, string type, DatesPeriodModel dates, string status)
            : base(type, dates, status)
        {
            this.CalendarEventId = calendarEventId;
        }
    }
}
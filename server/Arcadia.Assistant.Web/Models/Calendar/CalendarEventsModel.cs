namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsModel
    {
        [Required]
        public CalendarEventType Type { get; set; }

        [Required]
        public DatesPeriodModel Dates { get; set; }

        [DefaultValue(CalendarEventStatus.Requested)]
        public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Requested;

        public CalendarEventsModel()
        {
        }

        public CalendarEventsModel(CalendarEventType type, DatesPeriodModel dates, CalendarEventStatus status)
        {
            this.Type = type;
            this.Dates = dates;
            this.Status = status;
        }
    }
}
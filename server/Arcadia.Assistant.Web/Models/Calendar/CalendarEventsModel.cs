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
        public DatesPeriod Dates { get; set; }

        [DefaultValue(CalendarEventStatus.Requested)]
        public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Requested;

        public CalendarEventsModel()
        {
        }

        public CalendarEventsModel(DateTime from, DateTime to)
        {
            this.Dates = new DatesPeriod()
                {
                    StartDate = from, EndDate = to
                };
        }
    }
}
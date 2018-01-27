namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class VacationModel
    {
        [Required]
        public DatesPeriod Period { get; set; }

        [DefaultValue(CalendarEventStatus.Requested)]
        public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Requested;

        public class WithId : VacationModel
        {
            [Required]
            public string VacationId { get; set; }
        }
    }
}
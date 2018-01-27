namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class WorktimeChangeModel
    {
        /// <summary>
        /// Can be either positive or negative
        /// </summary>
        [Required]
        public int AdditionalWorkHours { get; set; }

        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// For example, 0 or 4
        /// </summary>
        [Required]
        public int StartHour { get; set; }

        [DefaultValue(CalendarEventStatus.Requested)]
        public CalendarEventStatus Status { get; set; }
    }
}
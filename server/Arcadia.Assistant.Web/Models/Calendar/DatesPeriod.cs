namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class DatesPeriod
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Starting working hour index. Typically, 0 or 4.
        /// </summary>
        [DefaultValue(0)]
        public int StartWorkingHour { get; set; } = 0;

        /// <summary>
        /// Finish working hour index. Typically, 4 or 8
        /// </summary>
        [DefaultValue(8)]
        public int FinishWorkingHour { get; set; } = 8;
    }
}
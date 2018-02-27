namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class DatesPeriodModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Starting working hour index. Typically, 0 or 4.
        /// </summary>
        [DefaultValue(0)]
        [Range(0, 23)]
        public int StartWorkingHour { get; set; } = 0;

        /// <summary>
        /// Finish working hour index. Typically, 4 or 8
        /// </summary>
        [DefaultValue(8)]
        [Range(1, 24)]
        public int FinishWorkingHour { get; set; } = 8;


        public static implicit operator DatesPeriod(DatesPeriodModel period)
        {
            return new DatesPeriod(period.StartDate, period.EndDate, period.StartWorkingHour, period.FinishWorkingHour);
        }

        public static implicit operator DatesPeriodModel(DatesPeriod period)
        {
            return new DatesPeriodModel()
                {
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    StartWorkingHour = period.StartWorkingHour,
                    FinishWorkingHour = period.FinishWorkingHour
                };
        }
    }
}
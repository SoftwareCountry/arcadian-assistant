namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;

    public class DatesPeriod
    {
        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        /// <summary>
        /// Starting working hour index. Typically, 0 or 4.
        /// </summary>
        public int StartWorkingHour { get; }

        /// <summary>
        /// Finish working hour index. Typically, 4 or 8
        /// </summary>
        public int FinishWorkingHour { get; }

        public DatesPeriod(DateTime startDate, DateTime endDate, int startWorkingHour = 0, int finishWorkingHour = 8)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.StartWorkingHour = startWorkingHour;
            this.FinishWorkingHour = finishWorkingHour;
        }
    }
}
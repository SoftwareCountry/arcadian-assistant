namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class WorktimeChange
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

        public class WithId : WorktimeChange
        {
            [Required]
            public string WorktimeChangeId { get; set; }
        }
    }
}
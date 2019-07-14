﻿namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StatusChange
    {
        [Required]
        public string ChangedByEmployeeId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        public string Comment { get; set; }

        [Required]
        public string EmployeeId { get; set; }


        [Required]
        public Guid ChangeRequestId { get; set; }

        [Required]
        [ForeignKey(nameof(EmployeeId) + "," + nameof(ChangeRequestId))]
        public virtual ChangeRequest ChangeRequest { get; set; }
    }
}
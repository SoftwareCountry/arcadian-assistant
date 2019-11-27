namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using Contracts;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ChangeRequest
    {
        public int EmployeeId { get; set; }

        public Guid ChangeRequestId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public WorkHoursChangeType ChangeType { get; set; }

        [Required]
        public DayPart DayPart { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        public virtual List<Approval> Approvals { get; set; } = new List<Approval>();

        public virtual List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

        public virtual List<Rejection> Rejections { get; set; } = new List<Rejection>();
    }
}
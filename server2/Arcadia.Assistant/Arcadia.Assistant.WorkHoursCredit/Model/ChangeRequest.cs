namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Contracts;

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

        public virtual List<Approval> Approvals { get; set; }

        public virtual List<Cancellation> Cancellations { get; set; }

        public virtual List<Rejection> Rejections { get; set; }
    }
}
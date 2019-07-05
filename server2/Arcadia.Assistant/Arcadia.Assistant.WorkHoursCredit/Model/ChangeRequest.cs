namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Contracts;

    public class ChangeRequest
    {
        [Key]
        public Guid ChangeRequestId { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public WorkHoursChangeType ChangeType { get; set; }

        [Required]
        public DayPart DayPart { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        public List<Approval> Approvals { get; set; }

        public List<Cancellation> Cancellations { get; set; }

        public List<Rejection> Rejections { get; set; }
    }
}
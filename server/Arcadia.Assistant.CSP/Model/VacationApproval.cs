namespace Arcadia.Assistant.CSP.Model
{
    using System;

    public partial class VacationApproval
    {
        public int Id { get; set; }

        public int VacationId { get; set; }

        public int ApproverId { get; set; }

        public int? NextApprovalId { get; set; }

        public int Status { get; set; }

        public DateTimeOffset? TimeStamp { get; set; }

        public Vacation Vacation { get; set; }

        public Employee Approver { get; set; }

        public Employee NextApproval { get; set; }
    }
}
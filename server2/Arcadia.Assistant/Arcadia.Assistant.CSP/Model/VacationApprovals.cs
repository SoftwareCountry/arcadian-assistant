using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationApprovals
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ApproverId { get; set; }
        public int Status { get; set; }
        public bool IsFinal { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }

        public Employee Approver { get; set; }
        public Vacations Vacation { get; set; }
    }
}

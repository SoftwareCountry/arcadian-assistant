using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationApproval
    {
        public VacationApproval()
        {
            InverseNextApproval = new HashSet<VacationApproval>();
        }

        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ApproverId { get; set; }
        public int? NextApprovalId { get; set; }
        public int Status { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }

        public Employee Approver { get; set; }
        public VacationApproval NextApproval { get; set; }
        public Vacation Vacation { get; set; }
        public ICollection<VacationApproval> InverseNextApproval { get; set; }
    }
}

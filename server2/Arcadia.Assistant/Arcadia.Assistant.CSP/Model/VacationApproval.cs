using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationApproval
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ApproverId { get; set; }
        public int Status { get; set; }
        public bool IsFinal { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }

        [ForeignKey("ApproverId")]
        [InverseProperty("VacationApprovals")]
        public virtual Employee Approver { get; set; }
        [ForeignKey("VacationId")]
        [InverseProperty("VacationApprovals")]
        public virtual Vacation Vacation { get; set; }
    }
}
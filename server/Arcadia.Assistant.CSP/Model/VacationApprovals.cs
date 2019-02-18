using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationApprovals
    {
        public VacationApprovals()
        {
            InverseNextApproval = new HashSet<VacationApprovals>();
            NotificationsVacationApproval = new HashSet<Notifications>();
            NotificationsVacationApprovalId1Navigation = new HashSet<Notifications>();
            NotificationsVacationApprovalId2Navigation = new HashSet<Notifications>();
        }

        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ApproverId { get; set; }
        public int? NextApprovalId { get; set; }
        public int Status { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }

        public Employee Approver { get; set; }
        public VacationApprovals NextApproval { get; set; }
        public Vacations Vacation { get; set; }
        public ICollection<VacationApprovals> InverseNextApproval { get; set; }
        public ICollection<Notifications> NotificationsVacationApproval { get; set; }
        public ICollection<Notifications> NotificationsVacationApprovalId1Navigation { get; set; }
        public ICollection<Notifications> NotificationsVacationApprovalId2Navigation { get; set; }
    }
}

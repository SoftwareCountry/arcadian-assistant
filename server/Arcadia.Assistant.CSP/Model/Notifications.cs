using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Notifications
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsNew { get; set; }
        public bool ActionRequired { get; set; }
        public string Message { get; set; }
        public string Payload { get; set; }
        public int? VacationId { get; set; }
        public int? VacationId1 { get; set; }
        public int? VacationApprovalId { get; set; }
        public int? VacationApprovalId1 { get; set; }
        public int? VacationApprovalId2 { get; set; }
        public string NotificationType { get; set; }

        public Employee Employee { get; set; }
        public Vacations Vacation { get; set; }
        public VacationApprovals VacationApproval { get; set; }
        public VacationApprovals VacationApprovalId1Navigation { get; set; }
        public VacationApprovals VacationApprovalId2Navigation { get; set; }
        public Vacations VacationId1Navigation { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class ForeignPassportHistory
    {
        public ForeignPassportHistory()
        {
            VisaHistory = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime Modified { get; set; }
        public bool HistoryFlag { get; set; }
        public DateTime? HistoryDate { get; set; }
        public int OriginId { get; set; }
        public int EmployeeHistoryId { get; set; }
        public bool IsDelete { get; set; }
        public string PassportNumber { get; set; }
        public string IssuedBy { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateOfExpiry { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Comment { get; set; }

        public EmployeeHistory EmployeeHistory { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public ForeignPassport Origin { get; set; }
        public ICollection<VisaHistory> VisaHistory { get; set; }
    }
}

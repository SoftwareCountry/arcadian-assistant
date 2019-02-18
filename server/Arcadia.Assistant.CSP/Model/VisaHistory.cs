using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VisaHistory
    {
        public int Id { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime Modified { get; set; }
        public bool HistoryFlag { get; set; }
        public DateTime? HistoryDate { get; set; }
        public int OriginId { get; set; }
        public int EmployeeHistoryId { get; set; }
        public bool IsDelete { get; set; }
        public int CountryId { get; set; }
        public string VisaNumber { get; set; }
        public string IssuedBy { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateOfExpiry { get; set; }
        public int ForeignPassportHistoryId { get; set; }
        public string MultiTime { get; set; }
        public string Days { get; set; }
        public DateTime? InsuranceStart { get; set; }
        public DateTime? InsuranceEnd { get; set; }

        public Country Country { get; set; }
        public EmployeeHistory EmployeeHistory { get; set; }
        public ForeignPassportHistory ForeignPassportHistory { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public Visa Origin { get; set; }
    }
}

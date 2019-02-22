using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Visa
    {
        public Visa()
        {
            VisaHistory = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDelete { get; set; }
        public int CountryId { get; set; }
        public string VisaNumber { get; set; }
        public string IssuedBy { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateOfExpiry { get; set; }
        public int ForeignPassportId { get; set; }
        public string MultiTime { get; set; }
        public string Days { get; set; }
        public DateTime? InsuranceStart { get; set; }
        public DateTime? InsuranceEnd { get; set; }

        public Country Country { get; set; }
        public Employee Employee { get; set; }
        public ForeignPassport ForeignPassport { get; set; }
        public ICollection<VisaHistory> VisaHistory { get; set; }
    }
}

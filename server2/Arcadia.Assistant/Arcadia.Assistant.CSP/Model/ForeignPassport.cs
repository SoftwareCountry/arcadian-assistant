using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class ForeignPassport
    {
        public ForeignPassport()
        {
            ForeignPassportHistory = new HashSet<ForeignPassportHistory>();
            Visa = new HashSet<Visa>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDelete { get; set; }
        public string PassportNumber { get; set; }
        public string IssuedBy { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateOfExpiry { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Comment { get; set; }

        public Employee Employee { get; set; }
        public ICollection<ForeignPassportHistory> ForeignPassportHistory { get; set; }
        public ICollection<Visa> Visa { get; set; }
    }
}

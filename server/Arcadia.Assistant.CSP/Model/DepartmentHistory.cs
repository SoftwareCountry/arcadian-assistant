using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class DepartmentHistory
    {
        public DepartmentHistory()
        {
            EmployeeHistory = new HashSet<EmployeeHistory>();
            InverseParentDepartmentHistory = new HashSet<DepartmentHistory>();
        }

        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public int DepartmentId { get; set; }
        public int ModifiedBy { get; set; }
        public string Abbreviation { get; set; }
        public int? ParentDepartmentHistoryId { get; set; }
        public string Description { get; set; }
        public int? ChiefId { get; set; }
        public bool IsProduction { get; set; }
        public string IntrabaseId { get; set; }
        public string Name { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessZip { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessStreet { get; set; }
        public string BusinessStreet2 { get; set; }
        public string BusinessStreet3 { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessFax { get; set; }
        public int? CompanyHistoryId { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? HistoryDate { get; set; }
        public bool? HistoryFlag { get; set; }

        public EmployeeHistory Chief { get; set; }
        public CompanyHistory CompanyHistory { get; set; }
        public Department Department { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public DepartmentHistory ParentDepartmentHistory { get; set; }
        public ICollection<EmployeeHistory> EmployeeHistory { get; set; }
        public ICollection<DepartmentHistory> InverseParentDepartmentHistory { get; set; }
    }
}

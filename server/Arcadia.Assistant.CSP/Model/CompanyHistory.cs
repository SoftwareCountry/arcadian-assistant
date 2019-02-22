using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class CompanyHistory
    {
        public CompanyHistory()
        {
            DepartmentHistory = new HashSet<DepartmentHistory>();
            EmployeeHistory = new HashSet<EmployeeHistory>();
        }

        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public int CompanyId { get; set; }
        public int ModifiedBy { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string IntrabaseId { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsCustomer { get; set; }
        public bool? IsVendor { get; set; }
        public bool? IsPartner { get; set; }
        public bool? IsManufacturer { get; set; }
        public bool? IsPublisher { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessZip { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessStreet { get; set; }
        public string BusinessStreet2 { get; set; }
        public string BusinessStreet3 { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessPhone2 { get; set; }
        public string BusinessFax { get; set; }
        public string Email { get; set; }
        public string PostAddress { get; set; }
        public string Web { get; set; }
        public string ContactName { get; set; }
        public string TypeOfBusiness { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }

        public Company Company { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public ICollection<DepartmentHistory> DepartmentHistory { get; set; }
        public ICollection<EmployeeHistory> EmployeeHistory { get; set; }
    }
}

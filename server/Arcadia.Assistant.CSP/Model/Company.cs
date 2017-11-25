using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Company
    {
        public Company()
        {
            this.CompanyHistory = new HashSet<CompanyHistory>();
            this.Department = new HashSet<Department>();
            this.Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
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
        public bool? IsDelete { get; set; }

        public ICollection<CompanyHistory> CompanyHistory { get; set; }
        public ICollection<Department> Department { get; set; }
        public ICollection<Employee> Employee { get; set; }
    }
}

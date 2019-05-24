using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Department
    {
        public Department()
        {
            DepartmentHistory = new HashSet<DepartmentHistory>();
            Employee = new HashSet<Employee>();
            InverseParentDepartment = new HashSet<Department>();
        }

        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public int? ParentDepartmentId { get; set; }
        public string Description { get; set; }
        public int? ChiefId { get; set; }
        public bool IsProduction { get; set; }
        public string IntrabaseId { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessZip { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessStreet { get; set; }
        public string BusinessStreet2 { get; set; }
        public string BusinessStreet3 { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessFax { get; set; }
        public bool IsDelete { get; set; }

        public Employee Chief { get; set; }
        public Company Company { get; set; }
        public Department ParentDepartment { get; set; }
        public ICollection<DepartmentHistory> DepartmentHistory { get; set; }
        public ICollection<Employee> Employee { get; set; }
        public ICollection<Department> InverseParentDepartment { get; set; }
    }
}

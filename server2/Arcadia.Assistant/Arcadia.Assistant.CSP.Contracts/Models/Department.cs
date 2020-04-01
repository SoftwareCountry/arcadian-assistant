namespace Arcadia.Assistant.CSP.Models
{
    using System.Collections.Generic;

    public class Department
    {
        public int Id { get; set; }

        public string Abbreviation { get; set; }

        public int? ParentDepartmentId { get; set; }

        public string Description { get; set; }

        public int? ChiefId { get; set; }

        /*
        public bool IsProduction { get; set; }
        */
        public string IntrabaseId { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        /*
        public string BusinessCountry { get; set; }
        public string BusinessZip { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessStreet { get; set; }
        public string BusinessStreet2 { get; set; }
        public string BusinessStreet3 { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessFax { get; set; }
        */
        public bool IsDelete { get; set; }

        public virtual Employee Chief { get; set; }

        //public virtual Company Company { get; set; }
        public virtual Department ParentDepartment { get; set; }
        //public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        public virtual ICollection<Department> InverseParentDepartment { get; set; } = new HashSet<Department>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("Department")]
    public partial class Department
    {
        public Department()
        {
            DepartmentHistories = new HashSet<DepartmentHistory>();
            Employees = new HashSet<Employee>();
            InverseParentDepartment = new HashSet<Department>();
        }

        public int Id { get; set; }
        [StringLength(40)]
        public string Abbreviation { get; set; }
        public int? ParentDepartmentId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int? ChiefId { get; set; }
        public bool IsProduction { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        [StringLength(40)]
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        [StringLength(80)]
        public string BusinessCountry { get; set; }
        [Column("BusinessZIP")]
        [StringLength(6)]
        public string BusinessZip { get; set; }
        [StringLength(100)]
        public string BusinessCity { get; set; }
        [StringLength(255)]
        public string BusinessStreet { get; set; }
        [StringLength(255)]
        public string BusinessStreet2 { get; set; }
        [StringLength(255)]
        public string BusinessStreet3 { get; set; }
        [StringLength(80)]
        public string BusinessPhone { get; set; }
        [StringLength(40)]
        public string BusinessFax { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("ChiefId")]
        [InverseProperty("Departments")]
        public virtual Employee Chief { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("Departments")]
        public virtual Company Company { get; set; }
        [ForeignKey("ParentDepartmentId")]
        [InverseProperty("InverseParentDepartment")]
        public virtual Department ParentDepartment { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<Employee> Employees { get; set; }
        [InverseProperty("ParentDepartment")]
        public virtual ICollection<Department> InverseParentDepartment { get; set; }
    }
}
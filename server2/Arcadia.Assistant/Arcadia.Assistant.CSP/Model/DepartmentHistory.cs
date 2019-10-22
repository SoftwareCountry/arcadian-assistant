using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("DepartmentHistory")]
    public partial class DepartmentHistory
    {
        public DepartmentHistory()
        {
            EmployeeHistories = new HashSet<EmployeeHistory>();
            InverseParentDepartmentHistory = new HashSet<DepartmentHistory>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int DepartmentId { get; set; }
        public int ModifiedBy { get; set; }
        [StringLength(40)]
        public string Abbreviation { get; set; }
        public int? ParentDepartmentHistoryId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int? ChiefId { get; set; }
        public bool IsProduction { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        [StringLength(40)]
        public string Name { get; set; }
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
        public int? CompanyHistoryId { get; set; }
        public bool IsDelete { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }

        [ForeignKey("ChiefId")]
        [InverseProperty("DepartmentHistories")]
        public virtual EmployeeHistory Chief { get; set; }
        [ForeignKey("CompanyHistoryId")]
        [InverseProperty("DepartmentHistories")]
        public virtual CompanyHistory CompanyHistory { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("DepartmentHistories")]
        public virtual Department Department { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("DepartmentHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [ForeignKey("ParentDepartmentHistoryId")]
        [InverseProperty("InverseParentDepartmentHistory")]
        public virtual DepartmentHistory ParentDepartmentHistory { get; set; }
        [InverseProperty("Department")]
        public virtual ICollection<EmployeeHistory> EmployeeHistories { get; set; }
        [InverseProperty("ParentDepartmentHistory")]
        public virtual ICollection<DepartmentHistory> InverseParentDepartmentHistory { get; set; }
    }
}
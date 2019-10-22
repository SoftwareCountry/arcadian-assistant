using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("CompanyHistory")]
    public partial class CompanyHistory
    {
        public CompanyHistory()
        {
            DepartmentHistories = new HashSet<DepartmentHistory>();
            EmployeeHistories = new HashSet<EmployeeHistory>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int CompanyId { get; set; }
        public int ModifiedBy { get; set; }
        [StringLength(120)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string ShortName { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsCustomer { get; set; }
        public bool? IsVendor { get; set; }
        public bool? IsPartner { get; set; }
        public bool? IsManufacturer { get; set; }
        public bool? IsPublisher { get; set; }
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
        [StringLength(80)]
        public string BusinessPhone2 { get; set; }
        [StringLength(40)]
        public string BusinessFax { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(255)]
        public string PostAddress { get; set; }
        [StringLength(255)]
        public string Web { get; set; }
        [StringLength(80)]
        public string ContactName { get; set; }
        public string TypeOfBusiness { get; set; }
        public bool IsDelete { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("CompanyHistories")]
        public virtual Company Company { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("CompanyHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [InverseProperty("CompanyHistory")]
        public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeHistory> EmployeeHistories { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("VisaHistory")]
    public partial class VisaHistory
    {
        public int Id { get; set; }
        public int ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public bool HistoryFlag { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public int OriginId { get; set; }
        public int EmployeeHistoryId { get; set; }
        public bool IsDelete { get; set; }
        public int CountryId { get; set; }
        [Required]
        [StringLength(50)]
        public string VisaNumber { get; set; }
        [StringLength(200)]
        public string IssuedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateOfIssue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateOfExpiry { get; set; }
        public int ForeignPassportHistoryId { get; set; }
        [StringLength(50)]
        public string MultiTime { get; set; }
        [StringLength(50)]
        public string Days { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsuranceStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsuranceEnd { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("VisaHistories")]
        public virtual Country Country { get; set; }
        [ForeignKey("EmployeeHistoryId")]
        [InverseProperty("VisaHistories")]
        public virtual EmployeeHistory EmployeeHistory { get; set; }
        [ForeignKey("ForeignPassportHistoryId")]
        [InverseProperty("VisaHistories")]
        public virtual ForeignPassportHistory ForeignPassportHistory { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("VisaHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [ForeignKey("OriginId")]
        [InverseProperty("VisaHistories")]
        public virtual Visa Origin { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("Visa")]
    public partial class Visa
    {
        public Visa()
        {
            VisaHistories = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
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
        public int ForeignPassportId { get; set; }
        [StringLength(50)]
        public string MultiTime { get; set; }
        [StringLength(50)]
        public string Days { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsuranceStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InsuranceEnd { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("Visas")]
        public virtual Country Country { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("Visas")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ForeignPassportId")]
        [InverseProperty("Visas")]
        public virtual ForeignPassport ForeignPassport { get; set; }
        [InverseProperty("Origin")]
        public virtual ICollection<VisaHistory> VisaHistories { get; set; }
    }
}
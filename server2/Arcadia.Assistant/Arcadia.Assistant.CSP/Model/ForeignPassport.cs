using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("ForeignPassport")]
    public partial class ForeignPassport
    {
        public ForeignPassport()
        {
            ForeignPassportHistories = new HashSet<ForeignPassportHistory>();
            Visas = new HashSet<Visa>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDelete { get; set; }
        [Required]
        [StringLength(50)]
        public string PassportNumber { get; set; }
        [Required]
        [StringLength(200)]
        public string IssuedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateOfIssue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateOfExpiry { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public string Comment { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("ForeignPassports")]
        public virtual Employee Employee { get; set; }
        [InverseProperty("Origin")]
        public virtual ICollection<ForeignPassportHistory> ForeignPassportHistories { get; set; }
        [InverseProperty("ForeignPassport")]
        public virtual ICollection<Visa> Visas { get; set; }
    }
}
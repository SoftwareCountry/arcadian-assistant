using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("ForeignPassportHistory")]
    public partial class ForeignPassportHistory
    {
        public ForeignPassportHistory()
        {
            VisaHistories = new HashSet<VisaHistory>();
        }

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

        [ForeignKey("EmployeeHistoryId")]
        [InverseProperty("ForeignPassportHistories")]
        public virtual EmployeeHistory EmployeeHistory { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("ForeignPassportHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [ForeignKey("OriginId")]
        [InverseProperty("ForeignPassportHistories")]
        public virtual ForeignPassport Origin { get; set; }
        [InverseProperty("ForeignPassportHistory")]
        public virtual ICollection<VisaHistory> VisaHistories { get; set; }
    }
}
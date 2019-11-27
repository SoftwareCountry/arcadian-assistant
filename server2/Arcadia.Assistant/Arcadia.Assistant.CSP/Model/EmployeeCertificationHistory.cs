using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeeCertificationHistory")]
    public partial class EmployeeCertificationHistory
    {
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int EmployeeCertificationId { get; set; }
        public int ModifiedBy { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }
        public bool IsDelete { get; set; }
        public int Type { get; set; }
        [Required]
        [StringLength(80)]
        public string Title { get; set; }
        [StringLength(80)]
        public string Degree { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PassDate { get; set; }
        [Column("EmployeeModifiedBy_Id")]
        public int? EmployeeModifiedById { get; set; }
        [StringLength(256)]
        public string Location { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }

        [ForeignKey("EmployeeCertificationId")]
        [InverseProperty("EmployeeCertificationHistories")]
        public virtual EmployeeCertification EmployeeCertification { get; set; }
        [ForeignKey("EmployeeModifiedById")]
        [InverseProperty("EmployeeCertificationHistories")]
        public virtual Employee EmployeeModifiedBy { get; set; }
    }
}
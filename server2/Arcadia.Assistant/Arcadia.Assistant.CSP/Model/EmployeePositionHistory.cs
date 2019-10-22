using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeePositionHistory")]
    public partial class EmployeePositionHistory
    {
        public EmployeePositionHistory()
        {
            EmployeeHistories = new HashSet<EmployeeHistory>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int EmployeePositionId { get; set; }
        public int ModifiedBy { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }
        [Required]
        [StringLength(50)]
        public string TitleShort { get; set; }
        [Required]
        [StringLength(50)]
        public string TitleRus { get; set; }

        [ForeignKey("EmployeePositionId")]
        [InverseProperty("EmployeePositionHistories")]
        public virtual EmployeePosition EmployeePosition { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("EmployeePositionHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [InverseProperty("Position")]
        public virtual ICollection<EmployeeHistory> EmployeeHistories { get; set; }
    }
}
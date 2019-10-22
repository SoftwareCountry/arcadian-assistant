using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("TeamHistory")]
    public partial class TeamHistory
    {
        public TeamHistory()
        {
            EmployeeTeamHistories = new HashSet<EmployeeTeamHistory>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int TeamId { get; set; }
        public int ModifiedBy { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public int? LeadHistoryId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        public bool IsDelete { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }

        [ForeignKey("LeadHistoryId")]
        [InverseProperty("TeamHistories")]
        public virtual EmployeeHistory LeadHistory { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("TeamHistories")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [ForeignKey("TeamId")]
        [InverseProperty("TeamHistories")]
        public virtual Team Team { get; set; }
        [InverseProperty("TeamHistory")]
        public virtual ICollection<EmployeeTeamHistory> EmployeeTeamHistories { get; set; }
    }
}
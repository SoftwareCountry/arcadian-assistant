using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("Team")]
    public partial class Team
    {
        public Team()
        {
            EmployeeTeams = new HashSet<EmployeeTeam>();
            TeamHistories = new HashSet<TeamHistory>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public int? LeadId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        public bool IsDelete { get; set; }

        [ForeignKey("LeadId")]
        [InverseProperty("Teams")]
        public virtual Employee Lead { get; set; }
        [InverseProperty("Team")]
        public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; }
        [InverseProperty("Team")]
        public virtual ICollection<TeamHistory> TeamHistories { get; set; }
    }
}
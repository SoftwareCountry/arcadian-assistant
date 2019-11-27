using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeeTeamHistory")]
    public partial class EmployeeTeamHistory
    {
        public int TeamHistoryId { get; set; }
        public int EmpolyeeHistoryId { get; set; }

        [ForeignKey("EmpolyeeHistoryId")]
        [InverseProperty("EmployeeTeamHistories")]
        public virtual EmployeeHistory EmpolyeeHistory { get; set; }
        [ForeignKey("TeamHistoryId")]
        [InverseProperty("EmployeeTeamHistories")]
        public virtual TeamHistory TeamHistory { get; set; }
    }
}
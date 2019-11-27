using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeeTeam")]
    public partial class EmployeeTeam
    {
        public int EmpolyeeId { get; set; }
        public int TeamId { get; set; }

        [ForeignKey("EmpolyeeId")]
        [InverseProperty("EmployeeTeams")]
        public virtual Employee Empolyee { get; set; }
        [ForeignKey("TeamId")]
        [InverseProperty("EmployeeTeams")]
        public virtual Team Team { get; set; }
    }
}
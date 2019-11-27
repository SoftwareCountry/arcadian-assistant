using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeRole
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeRoles")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("EmployeeRoles")]
        public virtual Csprole Role { get; set; }
    }
}
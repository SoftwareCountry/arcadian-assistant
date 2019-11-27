using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeePosition")]
    public partial class EmployeePosition
    {
        public EmployeePosition()
        {
            EmployeePositionHistories = new HashSet<EmployeePositionHistory>();
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        public bool IsDelete { get; set; }
        [Required]
        [StringLength(50)]
        public string TitleShort { get; set; }
        [Required]
        [StringLength(50)]
        public string TitleRus { get; set; }

        [InverseProperty("EmployeePosition")]
        public virtual ICollection<EmployeePositionHistory> EmployeePositionHistories { get; set; }
        [InverseProperty("Position")]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
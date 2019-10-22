using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("CSPRoles")]
    public partial class Csprole
    {
        public Csprole()
        {
            EmployeeRoles = new HashSet<EmployeeRole>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
    }
}
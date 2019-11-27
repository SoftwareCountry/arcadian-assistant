using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("CSPAlertType")]
    public partial class CspalertType
    {
        public CspalertType()
        {
            Cspalerts = new HashSet<Cspalert>();
            EmployeeCspalerts = new HashSet<EmployeeCspalert>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string AlertType { get; set; }
        public bool IsFixedDate { get; set; }
        [StringLength(50)]
        public string DisplayName { get; set; }

        [InverseProperty("AlertType")]
        public virtual ICollection<Cspalert> Cspalerts { get; set; }
        [InverseProperty("Cspalert")]
        public virtual ICollection<EmployeeCspalert> EmployeeCspalerts { get; set; }
    }
}
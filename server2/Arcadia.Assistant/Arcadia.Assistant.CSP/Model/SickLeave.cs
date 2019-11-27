using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class SickLeave
    {
        public SickLeave()
        {
            SickLeaveCancellations = new HashSet<SickLeaveCancellation>();
            SickLeaveCompletes = new HashSet<SickLeaveComplete>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Start { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime End { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("SickLeaves")]
        public virtual Employee Employee { get; set; }
        [InverseProperty("SickLeave")]
        public virtual ICollection<SickLeaveCancellation> SickLeaveCancellations { get; set; }
        [InverseProperty("SickLeave")]
        public virtual ICollection<SickLeaveComplete> SickLeaveCompletes { get; set; }
    }
}
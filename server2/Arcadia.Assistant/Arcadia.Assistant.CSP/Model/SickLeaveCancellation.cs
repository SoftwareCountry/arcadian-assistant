using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class SickLeaveCancellation
    {
        public int Id { get; set; }
        public int SickLeaveId { get; set; }
        public int ById { get; set; }
        public DateTimeOffset At { get; set; }

        [ForeignKey("ById")]
        [InverseProperty("SickLeaveCancellations")]
        public virtual Employee By { get; set; }
        [ForeignKey("SickLeaveId")]
        [InverseProperty("SickLeaveCancellations")]
        public virtual SickLeave SickLeave { get; set; }
    }
}
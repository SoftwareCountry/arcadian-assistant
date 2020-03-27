using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Contracts.Models
{
    public partial class SickLeave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual ICollection<SickLeaveCancellation> SickLeaveCancellations { get; set; } = new HashSet<SickLeaveCancellation>();
        public virtual ICollection<SickLeaveComplete> SickLeaveCompletes { get; set; } = new HashSet<SickLeaveComplete>();
    }
}
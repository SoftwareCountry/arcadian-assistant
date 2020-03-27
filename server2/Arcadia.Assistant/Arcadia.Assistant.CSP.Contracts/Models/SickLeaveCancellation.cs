using System;

namespace Arcadia.Assistant.CSP.Contracts.Models
{
    public partial class SickLeaveCancellation
    {
        public int Id { get; set; }
        public int SickLeaveId { get; set; }
        public int ById { get; set; }
        public DateTimeOffset At { get; set; }

        public virtual Employee By { get; set; }
        public virtual SickLeave SickLeave { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class SickLeaveCompletes
    {
        public int Id { get; set; }
        public int SickLeaveId { get; set; }
        public int ById { get; set; }
        public DateTimeOffset At { get; set; }

        public Employee By { get; set; }
        public SickLeaves SickLeave { get; set; }
    }
}

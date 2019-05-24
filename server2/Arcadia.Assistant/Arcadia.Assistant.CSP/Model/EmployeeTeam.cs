using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeTeam
    {
        public int EmpolyeeId { get; set; }
        public int TeamId { get; set; }

        public Employee Empolyee { get; set; }
        public Team Team { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeTeamHistory
    {
        public int TeamHistoryId { get; set; }
        public int EmpolyeeHistoryId { get; set; }

        public EmployeeHistory EmpolyeeHistory { get; set; }
        public TeamHistory TeamHistory { get; set; }
    }
}

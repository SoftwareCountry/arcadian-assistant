﻿using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Contracts.Models
{
    public partial class Vacation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Type { get; set; }
        public int? EmployeeId1 { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Employee EmployeeId1Navigation { get; set; }
        /*
        public virtual ICollection<VacationApproval> VacationApprovals { get; set; } = new HashSet<VacationApproval>();
        public virtual ICollection<VacationCancellation> VacationCancellations { get; set; }
        public virtual ICollection<VacationProcess> VacationProcesses { get; set; }
        public virtual ICollection<VacationReady> VacationReadies { get; set; }
        */
    }
}
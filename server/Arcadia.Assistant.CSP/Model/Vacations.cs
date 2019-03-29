using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Vacations
    {
        public Vacations()
        {
            VacationApprovals = new HashSet<VacationApprovals>();
            VacationCancellations = new HashSet<VacationCancellations>();
            VacationProcesses = new HashSet<VacationProcesses>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Type { get; set; }
        public int? EmployeeId1 { get; set; }

        public Employee Employee { get; set; }
        public Employee EmployeeId1Navigation { get; set; }
        public ICollection<VacationApprovals> VacationApprovals { get; set; }
        public ICollection<VacationCancellations> VacationCancellations { get; set; }
        public ICollection<VacationProcesses> VacationProcesses { get; set; }
    }
}

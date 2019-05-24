using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class SickLeaves
    {
        public SickLeaves()
        {
            SickLeaveAccepts = new HashSet<SickLeaveAccepts>();
            SickLeaveCancellations = new HashSet<SickLeaveCancellations>();
            SickLeaveCompletes = new HashSet<SickLeaveCompletes>();
            SickLeaveRejects = new HashSet<SickLeaveRejects>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Employee Employee { get; set; }
        public ICollection<SickLeaveAccepts> SickLeaveAccepts { get; set; }
        public ICollection<SickLeaveCancellations> SickLeaveCancellations { get; set; }
        public ICollection<SickLeaveCompletes> SickLeaveCompletes { get; set; }
        public ICollection<SickLeaveRejects> SickLeaveRejects { get; set; }
    }
}

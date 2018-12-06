namespace Arcadia.Assistant.CSP.Model
{
    using System;
    using System.Collections.Generic;

    public partial class Vacation
    {
        public Vacation()
        {
            this.VacationApprovals = new HashSet<VacationApproval>();
        }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTimeOffset RaisedAt { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int Type { get; set; }

        public int? CancelledById { get; set; }

        public DateTimeOffset? CancelledAt { get; set; }

        public Employee Employee { get; set; }

        public Employee CancelledBy { get; set; }

        public ICollection<VacationApproval> VacationApprovals { get; set; }
    }
}
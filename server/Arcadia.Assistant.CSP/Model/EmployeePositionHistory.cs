using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeePositionHistory
    {
        public EmployeePositionHistory()
        {
            EmployeeHistory = new HashSet<EmployeeHistory>();
        }

        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public int EmployeePositionId { get; set; }
        public int ModifiedBy { get; set; }
        public string Title { get; set; }
        public string IntrabaseId { get; set; }
        public string Description { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? HistoryDate { get; set; }
        public bool? HistoryFlag { get; set; }

        public EmployeePosition EmployeePosition { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public ICollection<EmployeeHistory> EmployeeHistory { get; set; }
    }
}

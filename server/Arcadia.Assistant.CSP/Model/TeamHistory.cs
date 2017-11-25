using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class TeamHistory
    {
        public TeamHistory()
        {
            EmployeeTeamHistory = new HashSet<EmployeeTeamHistory>();
        }

        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public int TeamId { get; set; }
        public int ModifiedBy { get; set; }
        public string Name { get; set; }
        public int? LeadHistoryId { get; set; }
        public string Description { get; set; }
        public string IntrabaseId { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? HistoryDate { get; set; }
        public bool? HistoryFlag { get; set; }

        public EmployeeHistory LeadHistory { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public Team Team { get; set; }
        public ICollection<EmployeeTeamHistory> EmployeeTeamHistory { get; set; }
    }
}

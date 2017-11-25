using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Team
    {
        public Team()
        {
            EmployeeTeam = new HashSet<EmployeeTeam>();
            TeamHistory = new HashSet<TeamHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? LeadId { get; set; }
        public string Description { get; set; }
        public string IntrabaseId { get; set; }
        public bool? IsDelete { get; set; }

        public Employee Lead { get; set; }
        public ICollection<EmployeeTeam> EmployeeTeam { get; set; }
        public ICollection<TeamHistory> TeamHistory { get; set; }
    }
}

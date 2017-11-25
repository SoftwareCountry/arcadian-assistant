using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeePosition
    {
        public EmployeePosition()
        {
            Employee = new HashSet<Employee>();
            EmployeePositionHistory = new HashSet<EmployeePositionHistory>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IntrabaseId { get; set; }
        public bool? IsDelete { get; set; }

        public ICollection<Employee> Employee { get; set; }
        public ICollection<EmployeePositionHistory> EmployeePositionHistory { get; set; }
    }
}

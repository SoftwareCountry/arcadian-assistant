using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Contracts.Models
{
    public partial class EmployeePosition
    {
        public EmployeePosition()
        {
//            EmployeePositionHistories = new HashSet<EmployeePositionHistory>();
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IntrabaseId { get; set; }
        public bool IsDelete { get; set; }
        public string TitleShort { get; set; }
        public string TitleRus { get; set; }

//        public virtual ICollection<EmployeePositionHistory> EmployeePositionHistories { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
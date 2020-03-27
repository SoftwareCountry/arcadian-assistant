using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.CSP.Contracts
{
    using Models;

    public class ArcadiaCspContext
    {
        public ArcadiaCspContext(string options)
        {

        }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        public List<Department> Departments { get; set; } = new List<Department>();

        public List<SickLeave> SickLeaves { get; set; } = new List<SickLeave>();

        public List<Vacation> Vacations { get; set; } = new List<Vacation>();
    }
}

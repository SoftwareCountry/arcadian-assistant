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

        public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();

        public IEnumerable<Department> Departments { get; set; } = new List<Department>();
    }
}

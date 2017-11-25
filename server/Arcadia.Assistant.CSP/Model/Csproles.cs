using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Csproles
    {
        public Csproles()
        {
            EmployeeRoles = new HashSet<EmployeeRoles>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<EmployeeRoles> EmployeeRoles { get; set; }
    }
}

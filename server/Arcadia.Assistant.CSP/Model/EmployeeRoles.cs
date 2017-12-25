using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeRoles
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }

        public Employee Employee { get; set; }
        public Csproles Role { get; set; }
    }
}

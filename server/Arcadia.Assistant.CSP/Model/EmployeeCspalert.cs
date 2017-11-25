using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeCspalert
    {
        public int EmployeeId { get; set; }
        public int CspalertId { get; set; }
        public int? TimeAdvance { get; set; }

        public CspalertType Cspalert { get; set; }
        public Employee Employee { get; set; }
    }
}

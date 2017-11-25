using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class CspalertType
    {
        public CspalertType()
        {
            Cspalert = new HashSet<Cspalert>();
            EmployeeCspalert = new HashSet<EmployeeCspalert>();
        }

        public int Id { get; set; }
        public string AlertType { get; set; }
        public bool IsFixedDate { get; set; }
        public string DisplayName { get; set; }

        public ICollection<Cspalert> Cspalert { get; set; }
        public ICollection<EmployeeCspalert> EmployeeCspalert { get; set; }
    }
}

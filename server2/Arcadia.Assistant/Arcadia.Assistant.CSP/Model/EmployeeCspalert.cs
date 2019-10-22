using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeeCSPAlert")]
    public partial class EmployeeCspalert
    {
        public int EmployeeId { get; set; }
        [Column("CSPAlertId")]
        public int CspalertId { get; set; }
        public int? TimeAdvance { get; set; }

        [ForeignKey("CspalertId")]
        [InverseProperty("EmployeeCspalerts")]
        public virtual CspalertType Cspalert { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeCspalerts")]
        public virtual Employee Employee { get; set; }
    }
}
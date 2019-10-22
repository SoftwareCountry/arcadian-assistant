using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Vacation
    {
        public Vacation()
        {
            VacationApprovals = new HashSet<VacationApproval>();
            VacationCancellations = new HashSet<VacationCancellation>();
            VacationProcesses = new HashSet<VacationProcess>();
            VacationReadies = new HashSet<VacationReady>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset RaisedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Start { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime End { get; set; }
        public int Type { get; set; }
        [Column("Employee_Id")]
        public int? EmployeeId1 { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("VacationEmployees")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("EmployeeId1")]
        [InverseProperty("VacationEmployeeId1Navigations")]
        public virtual Employee EmployeeId1Navigation { get; set; }
        [InverseProperty("Vacation")]
        public virtual ICollection<VacationApproval> VacationApprovals { get; set; }
        [InverseProperty("Vacation")]
        public virtual ICollection<VacationCancellation> VacationCancellations { get; set; }
        [InverseProperty("Vacation")]
        public virtual ICollection<VacationProcess> VacationProcesses { get; set; }
        [InverseProperty("Vacation")]
        public virtual ICollection<VacationReady> VacationReadies { get; set; }
    }
}
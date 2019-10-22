using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationProcess
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ProcessById { get; set; }
        public DateTimeOffset ProcessedAt { get; set; }

        [ForeignKey("ProcessById")]
        [InverseProperty("VacationProcesses")]
        public virtual Employee ProcessBy { get; set; }
        [ForeignKey("VacationId")]
        [InverseProperty("VacationProcesses")]
        public virtual Vacation Vacation { get; set; }
    }
}
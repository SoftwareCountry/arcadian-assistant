using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationReady
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ReadyById { get; set; }
        public DateTimeOffset ReadyAt { get; set; }

        [ForeignKey("ReadyById")]
        [InverseProperty("VacationReadies")]
        public virtual Employee ReadyBy { get; set; }
        [ForeignKey("VacationId")]
        [InverseProperty("VacationReadies")]
        public virtual Vacation Vacation { get; set; }
    }
}
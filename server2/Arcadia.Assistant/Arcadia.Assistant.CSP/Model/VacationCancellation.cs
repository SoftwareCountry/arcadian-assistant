using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationCancellation
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int CancelledById { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        [StringLength(1000)]
        public string Reason { get; set; }

        [ForeignKey("CancelledById")]
        [InverseProperty("VacationCancellations")]
        public virtual Employee CancelledBy { get; set; }
        [ForeignKey("VacationId")]
        [InverseProperty("VacationCancellations")]
        public virtual Vacation Vacation { get; set; }
    }
}
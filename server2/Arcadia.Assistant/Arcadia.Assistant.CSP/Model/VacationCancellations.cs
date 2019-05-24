using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationCancellations
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int CancelledById { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        public string Reason { get; set; }

        public Employee CancelledBy { get; set; }
        public Vacations Vacation { get; set; }
    }
}

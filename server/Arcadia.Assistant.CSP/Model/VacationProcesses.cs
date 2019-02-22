using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationProcesses
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ProcessById { get; set; }
        public DateTimeOffset ProcessedAt { get; set; }

        public Employee ProcessBy { get; set; }
        public Vacations Vacation { get; set; }
    }
}

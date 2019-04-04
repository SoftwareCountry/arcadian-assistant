using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class VacationReadies
    {
        public int Id { get; set; }
        public int VacationId { get; set; }
        public int ReadyById { get; set; }
        public DateTimeOffset ReadyAt { get; set; }

        public Employee ReadyBy { get; set; }
        public Vacations Vacation { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Holidays
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public short BusinessHours { get; set; }
        public string Description { get; set; }
    }
}

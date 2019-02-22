using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Rooms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoomNumber { get; set; }
        public decimal TotalArea { get; set; }
        public decimal EffectiveArea { get; set; }
        public int EthernetPlugs { get; set; }
    }
}

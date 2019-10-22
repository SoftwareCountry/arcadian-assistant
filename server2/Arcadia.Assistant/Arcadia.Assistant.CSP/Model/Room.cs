using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Room
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(20)]
        public string RoomNumber { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalArea { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal EffectiveArea { get; set; }
        public int EthernetPlugs { get; set; }
    }
}
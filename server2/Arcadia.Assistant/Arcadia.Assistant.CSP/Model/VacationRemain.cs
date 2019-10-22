using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("VacationRemain")]
    public partial class VacationRemain
    {
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdateDate { get; set; }
        public int Value { get; set; }

        [ForeignKey("Id")]
        [InverseProperty("VacationRemain")]
        public virtual Employee IdNavigation { get; set; }
    }
}
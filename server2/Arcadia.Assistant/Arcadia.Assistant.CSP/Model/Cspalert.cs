using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("CSPAlert")]
    public partial class Cspalert
    {
        public int AlertTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime AlertDate { get; set; }
        [Required]
        public string AlertText { get; set; }
        public int Id { get; set; }

        [ForeignKey("AlertTypeId")]
        [InverseProperty("Cspalerts")]
        public virtual CspalertType AlertType { get; set; }
    }
}
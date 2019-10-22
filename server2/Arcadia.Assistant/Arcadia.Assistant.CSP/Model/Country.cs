using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("Country")]
    public partial class Country
    {
        public Country()
        {
            VisaHistories = new HashSet<VisaHistory>();
            Visas = new HashSet<Visa>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string NameRus { get; set; }
        [Required]
        [StringLength(2)]
        public string Code2 { get; set; }
        [Required]
        [StringLength(3)]
        public string Code3 { get; set; }
        public bool IsSchengen { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<VisaHistory> VisaHistories { get; set; }
        [InverseProperty("Country")]
        public virtual ICollection<Visa> Visas { get; set; }
    }
}
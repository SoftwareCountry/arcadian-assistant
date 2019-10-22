using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeCertification
    {
        public EmployeeCertification()
        {
            CertificationData = new HashSet<CertificationDatum>();
            EmployeeCertificationHistories = new HashSet<EmployeeCertificationHistory>();
        }

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool IsDelete { get; set; }
        public int Type { get; set; }
        [Required]
        [StringLength(80)]
        public string Title { get; set; }
        [StringLength(80)]
        public string Degree { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime BeginDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PassDate { get; set; }
        [StringLength(256)]
        public string Location { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeCertifications")]
        public virtual Employee Employee { get; set; }
        [InverseProperty("EmployeeCertification")]
        public virtual ICollection<CertificationDatum> CertificationData { get; set; }
        [InverseProperty("EmployeeCertification")]
        public virtual ICollection<EmployeeCertificationHistory> EmployeeCertificationHistories { get; set; }
    }
}
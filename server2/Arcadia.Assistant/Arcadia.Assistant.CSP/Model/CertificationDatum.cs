using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class CertificationDatum
    {
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int? CreatedBy { get; set; }
        public int EmployeeCertificationId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public bool IsDelete { get; set; }
        public int DataType { get; set; }
        public string DataHead { get; set; }
        public byte[] Data { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("CertificationData")]
        public virtual Employee CreatedByNavigation { get; set; }
        [ForeignKey("EmployeeCertificationId")]
        [InverseProperty("CertificationData")]
        public virtual EmployeeCertification EmployeeCertification { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("EmployeeHistory")]
    public partial class EmployeeHistory
    {
        public EmployeeHistory()
        {
            DepartmentHistories = new HashSet<DepartmentHistory>();
            EmployeeTeamHistories = new HashSet<EmployeeTeamHistory>();
            ForeignPassportHistories = new HashSet<ForeignPassportHistory>();
            TeamHistories = new HashSet<TeamHistory>();
            VisaHistories = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public int EmployeeId { get; set; }
        public int ModifiedBy { get; set; }
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }
        [StringLength(20)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(40)]
        public string LastName { get; set; }
        [Required]
        [StringLength(20)]
        public string FirstNameRus { get; set; }
        [StringLength(20)]
        public string MiddleNameRus { get; set; }
        [Required]
        [StringLength(20)]
        public string LastNameRus { get; set; }
        [StringLength(40)]
        public string LoginName { get; set; }
        [Column("SID")]
        public Guid? Sid { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Birthday { get; set; }
        [StringLength(20)]
        public string BusinessPhone { get; set; }
        [StringLength(50)]
        public string HomePhone { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        [Column(TypeName = "date")]
        public DateTime HiringDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiringDate { get; set; }
        public bool IsWorking { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public bool IsPartTime { get; set; }
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
        public int? CompanyId { get; set; }
        public bool IsInStaff { get; set; }
        [StringLength(12)]
        public string IntrabaseId { get; set; }
        [Required]
        [StringLength(1)]
        public string Gender { get; set; }
        public short ClockNumber { get; set; }
        [StringLength(20)]
        public string RoomNumber { get; set; }
        [StringLength(80)]
        public string BusinessCountry { get; set; }
        [Column("BusinessZIP")]
        [StringLength(6)]
        public string BusinessZip { get; set; }
        [StringLength(100)]
        public string BusinessCity { get; set; }
        [StringLength(255)]
        public string BusinessStreet { get; set; }
        [StringLength(255)]
        public string BusinessStreet2 { get; set; }
        [StringLength(255)]
        public string BusinessStreet3 { get; set; }
        [StringLength(20)]
        public string BusinessPhone2 { get; set; }
        [StringLength(80)]
        public string HomeCountry { get; set; }
        [Column("HomeZIP")]
        [StringLength(6)]
        public string HomeZip { get; set; }
        [StringLength(100)]
        public string HomeCity { get; set; }
        [StringLength(255)]
        public string HomeStreet { get; set; }
        [StringLength(255)]
        public string HomeStreet2 { get; set; }
        [StringLength(255)]
        public string HomeStreet3 { get; set; }
        [StringLength(20)]
        public string MobilePhone { get; set; }
        [Column(TypeName = "date")]
        public DateTime ProbationEnd { get; set; }
        public int WeekHours { get; set; }
        public int? PartTime { get; set; }
        public bool IsDelete { get; set; }
        [Column(TypeName = "date")]
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }
        public bool IsInfOffice { get; set; }
        public bool IsPublishDeny { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeHistories")]
        public virtual CompanyHistory Company { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("EmployeeHistories")]
        public virtual DepartmentHistory Department { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeHistoryEmployees")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("ModifiedBy")]
        [InverseProperty("EmployeeHistoryModifiedByNavigations")]
        public virtual Employee ModifiedByNavigation { get; set; }
        [ForeignKey("PositionId")]
        [InverseProperty("EmployeeHistories")]
        public virtual EmployeePositionHistory Position { get; set; }
        [InverseProperty("Chief")]
        public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }
        [InverseProperty("EmpolyeeHistory")]
        public virtual ICollection<EmployeeTeamHistory> EmployeeTeamHistories { get; set; }
        [InverseProperty("EmployeeHistory")]
        public virtual ICollection<ForeignPassportHistory> ForeignPassportHistories { get; set; }
        [InverseProperty("LeadHistory")]
        public virtual ICollection<TeamHistory> TeamHistories { get; set; }
        [InverseProperty("EmployeeHistory")]
        public virtual ICollection<VisaHistory> VisaHistories { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcadia.Assistant.CSP.Model
{
    [Table("Employee")]
    public partial class Employee
    {
        public Employee()
        {
            CertificationData = new HashSet<CertificationDatum>();
            CompanyHistories = new HashSet<CompanyHistory>();
            DepartmentHistories = new HashSet<DepartmentHistory>();
            Departments = new HashSet<Department>();
            EmployeeCertificationHistories = new HashSet<EmployeeCertificationHistory>();
            EmployeeCertifications = new HashSet<EmployeeCertification>();
            EmployeeCspalerts = new HashSet<EmployeeCspalert>();
            EmployeeHistoryEmployees = new HashSet<EmployeeHistory>();
            EmployeeHistoryModifiedByNavigations = new HashSet<EmployeeHistory>();
            EmployeePositionHistories = new HashSet<EmployeePositionHistory>();
            EmployeeRoles = new HashSet<EmployeeRole>();
            EmployeeTeams = new HashSet<EmployeeTeam>();
            ForeignPassportHistories = new HashSet<ForeignPassportHistory>();
            ForeignPassports = new HashSet<ForeignPassport>();
            SickLeaveCancellations = new HashSet<SickLeaveCancellation>();
            SickLeaveCompletes = new HashSet<SickLeaveComplete>();
            SickLeaves = new HashSet<SickLeave>();
            TeamHistories = new HashSet<TeamHistory>();
            Teams = new HashSet<Team>();
            VacationApprovals = new HashSet<VacationApproval>();
            VacationCancellations = new HashSet<VacationCancellation>();
            VacationEmployeeId1Navigations = new HashSet<Vacation>();
            VacationEmployees = new HashSet<Vacation>();
            VacationProcesses = new HashSet<VacationProcess>();
            VacationReadies = new HashSet<VacationReady>();
            VisaHistories = new HashSet<VisaHistory>();
            Visas = new HashSet<Visa>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }
        [StringLength(20)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(20)]
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
        [StringLength(1000)]
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
        public bool IsInfOffice { get; set; }
        public bool IsPublishDeny { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("Employees")]
        public virtual Company Company { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("Employees")]
        public virtual Department Department { get; set; }
        [ForeignKey("PositionId")]
        [InverseProperty("Employees")]
        public virtual EmployeePosition Position { get; set; }
        [InverseProperty("IdNavigation")]
        public virtual VacationRemain VacationRemain { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<CertificationDatum> CertificationData { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<CompanyHistory> CompanyHistories { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }
        [InverseProperty("Chief")]
        public virtual ICollection<Department> Departments { get; set; }
        [InverseProperty("EmployeeModifiedBy")]
        public virtual ICollection<EmployeeCertificationHistory> EmployeeCertificationHistories { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeCertification> EmployeeCertifications { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeCspalert> EmployeeCspalerts { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeHistory> EmployeeHistoryEmployees { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<EmployeeHistory> EmployeeHistoryModifiedByNavigations { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<EmployeePositionHistory> EmployeePositionHistories { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        [InverseProperty("Empolyee")]
        public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<ForeignPassportHistory> ForeignPassportHistories { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<ForeignPassport> ForeignPassports { get; set; }
        [InverseProperty("By")]
        public virtual ICollection<SickLeaveCancellation> SickLeaveCancellations { get; set; }
        [InverseProperty("By")]
        public virtual ICollection<SickLeaveComplete> SickLeaveCompletes { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<SickLeave> SickLeaves { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<TeamHistory> TeamHistories { get; set; }
        [InverseProperty("Lead")]
        public virtual ICollection<Team> Teams { get; set; }
        [InverseProperty("Approver")]
        public virtual ICollection<VacationApproval> VacationApprovals { get; set; }
        [InverseProperty("CancelledBy")]
        public virtual ICollection<VacationCancellation> VacationCancellations { get; set; }
        [InverseProperty("EmployeeId1Navigation")]
        public virtual ICollection<Vacation> VacationEmployeeId1Navigations { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<Vacation> VacationEmployees { get; set; }
        [InverseProperty("ProcessBy")]
        public virtual ICollection<VacationProcess> VacationProcesses { get; set; }
        [InverseProperty("ReadyBy")]
        public virtual ICollection<VacationReady> VacationReadies { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<VisaHistory> VisaHistories { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<Visa> Visas { get; set; }
    }
}
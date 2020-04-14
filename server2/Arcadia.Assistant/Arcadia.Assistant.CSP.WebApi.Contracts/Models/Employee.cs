namespace Arcadia.Assistant.CSP.WebApi.Contracts.Models
{
    using System;
    //using System.Collections.Generic;

    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string MiddleName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullnameEng { get; set; } = string.Empty;

        public string FullnameRus { get; set; } = string.Empty;

        public string FirstNameRus { get; set; } = string.Empty;

        public string MiddleNameRus { get; set; } = string.Empty;

        public string LastNameRus { get; set; } = string.Empty;

        public string LoginName { get; set; } = string.Empty;

        public Guid? Sid { get; set; }

        public DateTime? Birthday { get; set; }

        /*
        public string BusinessPhone { get; set; }
        public string HomePhone { get; set; }
        */
        public string Email { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public int? PositionId { get; set; }

        public DateTime HiringDate { get; set; }

        public DateTime? FiringDate { get; set; }

        public int? ManagerId { get; set; }
        public int? VacationRemain { get; set; }

        /*
        public bool IsWorking { get; set; }
        public string Description { get; set; }
        public bool IsPartTime { get; set; }
        */
        public byte[]? Image { get; set; }

        public int? CompanyId { get; set; }

        /*
        public bool IsInStaff { get; set; }
        */
        //public string IntrabaseId { get; set; } = string.Empty;

        /*
        public string Gender { get; set; }
        public short ClockNumber { get; set; }
        */
        public string RoomNumber { get; set; } = string.Empty;

        /*
        public string BusinessCountry { get; set; }
        public string BusinessZip { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessStreet { get; set; }
        public string BusinessStreet2 { get; set; }
        public string BusinessStreet3 { get; set; }
        public string BusinessPhone2 { get; set; }
        public string HomeCountry { get; set; }
        public string HomeZip { get; set; }
        public string HomeCity { get; set; }
        public string HomeStreet { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeStreet3 { get; set; }
        */
        public string MobilePhone { get; set; } = string.Empty;
        /*
        public DateTime ProbationEnd { get; set; }
        public int WeekHours { get; set; }
        public int? PartTime { get; set; }
        */
        public bool IsDelete { get; set; }

        /*
        public bool IsInfOffice { get; set; }
        public bool IsPublishDeny { get; set; }
        */
        /*
        public virtual Company Company { get; set; }
        */

        //public virtual Department Department { get; set; }
        public string Department { get; set; } = string.Empty;

        //public virtual EmployeePosition Position { get; set; }
        public string Position { get; set; } = string.Empty;

        /*
        [InverseProperty("IdNavigation")]
        public virtual VacationRemain VacationRemain { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<CertificationDatum> CertificationData { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<CompanyHistory> CompanyHistories { get; set; }
        [InverseProperty("ModifiedByNavigation")]
        public virtual ICollection<DepartmentHistory> DepartmentHistories { get; set; }
        */
        //public virtual ICollection<Department> Departments { get; set; } = new HashSet<Department>();

        /*
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
        */
    }
}
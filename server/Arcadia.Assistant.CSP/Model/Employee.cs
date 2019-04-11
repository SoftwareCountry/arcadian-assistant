using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class Employee
    {
        public Employee()
        {
            CompanyHistory = new HashSet<CompanyHistory>();
            Department = new HashSet<Department>();
            DepartmentHistory = new HashSet<DepartmentHistory>();
            EmployeeCspalert = new HashSet<EmployeeCspalert>();
            EmployeeHistoryEmployee = new HashSet<EmployeeHistory>();
            EmployeeHistoryModifiedByNavigation = new HashSet<EmployeeHistory>();
            EmployeePositionHistory = new HashSet<EmployeePositionHistory>();
            EmployeeRoles = new HashSet<EmployeeRoles>();
            EmployeeTeam = new HashSet<EmployeeTeam>();
            ForeignPassport = new HashSet<ForeignPassport>();
            ForeignPassportHistory = new HashSet<ForeignPassportHistory>();
            SickLeaveAccepts = new HashSet<SickLeaveAccepts>();
            SickLeaveCancellations = new HashSet<SickLeaveCancellations>();
            SickLeaveCompletes = new HashSet<SickLeaveCompletes>();
            SickLeaveRejects = new HashSet<SickLeaveRejects>();
            SickLeaves = new HashSet<SickLeaves>();
            Team = new HashSet<Team>();
            TeamHistory = new HashSet<TeamHistory>();
            VacationApprovals = new HashSet<VacationApprovals>();
            VacationCancellations = new HashSet<VacationCancellations>();
            VacationProcesses = new HashSet<VacationProcesses>();
            VacationReadies = new HashSet<VacationReadies>();
            VacationsEmployee = new HashSet<Vacations>();
            VacationsEmployeeId1Navigation = new HashSet<Vacations>();
            Visa = new HashSet<Visa>();
            VisaHistory = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FirstNameRus { get; set; }
        public string MiddleNameRus { get; set; }
        public string LastNameRus { get; set; }
        public string LoginName { get; set; }
        public Guid? Sid { get; set; }
        public DateTime? Birthday { get; set; }
        public string BusinessPhone { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime HiringDate { get; set; }
        public DateTime? FiringDate { get; set; }
        public bool IsWorking { get; set; }
        public string Description { get; set; }
        public bool IsPartTime { get; set; }
        public byte[] Image { get; set; }
        public int? CompanyId { get; set; }
        public bool IsInStaff { get; set; }
        public string IntrabaseId { get; set; }
        public string Gender { get; set; }
        public short ClockNumber { get; set; }
        public string RoomNumber { get; set; }
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
        public string MobilePhone { get; set; }
        public DateTime ProbationEnd { get; set; }
        public int WeekHours { get; set; }
        public int? PartTime { get; set; }
        public bool IsDelete { get; set; }

        public Company Company { get; set; }
        public Department DepartmentNavigation { get; set; }
        public EmployeePosition Position { get; set; }
        public ICollection<CompanyHistory> CompanyHistory { get; set; }
        public ICollection<Department> Department { get; set; }
        public ICollection<DepartmentHistory> DepartmentHistory { get; set; }
        public ICollection<EmployeeCspalert> EmployeeCspalert { get; set; }
        public ICollection<EmployeeHistory> EmployeeHistoryEmployee { get; set; }
        public ICollection<EmployeeHistory> EmployeeHistoryModifiedByNavigation { get; set; }
        public ICollection<EmployeePositionHistory> EmployeePositionHistory { get; set; }
        public ICollection<EmployeeRoles> EmployeeRoles { get; set; }
        public ICollection<EmployeeTeam> EmployeeTeam { get; set; }
        public ICollection<ForeignPassport> ForeignPassport { get; set; }
        public ICollection<ForeignPassportHistory> ForeignPassportHistory { get; set; }
        public ICollection<SickLeaveAccepts> SickLeaveAccepts { get; set; }
        public ICollection<SickLeaveCancellations> SickLeaveCancellations { get; set; }
        public ICollection<SickLeaveCompletes> SickLeaveCompletes { get; set; }
        public ICollection<SickLeaveRejects> SickLeaveRejects { get; set; }
        public ICollection<SickLeaves> SickLeaves { get; set; }
        public ICollection<Team> Team { get; set; }
        public ICollection<TeamHistory> TeamHistory { get; set; }
        public ICollection<VacationApprovals> VacationApprovals { get; set; }
        public ICollection<VacationCancellations> VacationCancellations { get; set; }
        public ICollection<VacationProcesses> VacationProcesses { get; set; }
        public ICollection<VacationReadies> VacationReadies { get; set; }
        public ICollection<Vacations> VacationsEmployee { get; set; }
        public ICollection<Vacations> VacationsEmployeeId1Navigation { get; set; }
        public ICollection<Visa> Visa { get; set; }
        public ICollection<VisaHistory> VisaHistory { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class EmployeeHistory
    {
        public EmployeeHistory()
        {
            DepartmentHistory = new HashSet<DepartmentHistory>();
            EmployeeTeamHistory = new HashSet<EmployeeTeamHistory>();
            ForeignPassportHistory = new HashSet<ForeignPassportHistory>();
            TeamHistory = new HashSet<TeamHistory>();
            VisaHistory = new HashSet<VisaHistory>();
        }

        public int Id { get; set; }
        public DateTime Modified { get; set; }
        public int EmployeeId { get; set; }
        public int ModifiedBy { get; set; }
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
        public DateTime? HistoryDate { get; set; }
        public bool HistoryFlag { get; set; }

        public CompanyHistory Company { get; set; }
        public DepartmentHistory Department { get; set; }
        public Employee Employee { get; set; }
        public Employee ModifiedByNavigation { get; set; }
        public EmployeePositionHistory Position { get; set; }
        public ICollection<DepartmentHistory> DepartmentHistory { get; set; }
        public ICollection<EmployeeTeamHistory> EmployeeTeamHistory { get; set; }
        public ICollection<ForeignPassportHistory> ForeignPassportHistory { get; set; }
        public ICollection<TeamHistory> TeamHistory { get; set; }
        public ICollection<VisaHistory> VisaHistory { get; set; }
    }
}

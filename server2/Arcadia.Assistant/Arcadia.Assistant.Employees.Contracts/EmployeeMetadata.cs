namespace Arcadia.Assistant.Employees.Contracts
{
    using System;
    using System.Runtime.Serialization;

    public enum Sex { Male, Female, Undefined }

    [DataContract]
    public class EmployeeMetadata
    {
        public EmployeeMetadata(EmployeeId employeeId, string email)
        {
            this.EmployeeId = employeeId;
            this.Email = email;
        }

        [DataMember]
        public EmployeeId EmployeeId { get; private set; }

        [DataMember]
        public string? FirstName { get; set; }

        [DataMember]
        public string? LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string? RoomNumber { get; set; }

        [DataMember]
        public string? Position { get; set; }

        [DataMember]
        public string? MobilePhone { get; set; }

        [DataMember]
        public DepartmentId? DepartmentId { get; set; }

        [DataMember]
        public DateTime HireDate { get; set; }

        [DataMember]
        public DateTime? FireDate { get; set; }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        public Sex Sex { get; set; } = Sex.Undefined;

        public int? Age => CalculateYearsFromDate(this.BirthDate);

        public int? YearsServed => CalculateYearsFromDate(this.HireDate, this.FireDate);

        public int? AgeAt(DateTime date)
        {
            return CalculateYearsFromDate(this.BirthDate, date);
        }

        public int? YearsServedAt(DateTime date)
        {
            DateTime toDate;
            if (this.FireDate == null)
            {
                toDate = date;
            }
            else
            {
                toDate = date > this.FireDate ? this.FireDate.Value : date;
            }

            return CalculateYearsFromDate(this.HireDate, toDate);
        }

        private static int? CalculateYearsFromDate(DateTime? fromDate, DateTime? toDate = null)
        {
            if (fromDate == null)
            {
                return null;
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            var years = toDate.Value.Year - fromDate.Value.Year;

            if ((fromDate.Value.Month > toDate.Value.Month) || ((fromDate.Value.Month == toDate.Value.Month) && (fromDate.Value.Day > toDate.Value.Day)))
            {
                years = years - 1;
            }

            return years;
        }

        public override string ToString() => $"{this.LastName} {this.FirstName} <{this.EmployeeId}>, Email: {this.Email}, Department {this.DepartmentId}";
    }
}
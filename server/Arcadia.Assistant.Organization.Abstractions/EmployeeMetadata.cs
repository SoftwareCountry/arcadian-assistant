namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;
    using System.Diagnostics;

    public class EmployeeMetadata
    {
        public EmployeeMetadata(string employeeId, string name, string email)
        {
            this.EmployeeId = employeeId;
            this.Name = name;
            this.Email = email;
        }

        public string EmployeeId { get; }

        public string Name { get; }

        public string Email { get; }

        public string RoomNumber { get; set; }

        public string Position { get; set; }

        public string MobilePhone { get; set; }

        public string DepartmentId { get; set; }

        public string Sid { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime? FireDate { get; set; }

        public DateTime? BirthDate { get; set; }

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

            return CalculateYearsFromDate(toDate);
        }

        private static int? CalculateYearsFromDate(DateTime? fromDate, DateTime? toDate = null)
        {
            if (fromDate == null)
            {
                return null;
            }

            if (toDate == null) { 
                toDate = DateTime.Now;
            }

            var years = toDate.Value.Year - fromDate.Value.Year;
            if (toDate.Value.DayOfYear < fromDate.Value.DayOfYear)
            {
                years = years - 1;
            }

            return years;
        }

        public override string ToString() => $"{this.Name} <{this.EmployeeId}>, Email: {this.Email}";
    }
}
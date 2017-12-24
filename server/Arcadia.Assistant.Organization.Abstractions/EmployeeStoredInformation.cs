namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;

    public enum Sex { Male, Female, Undefined }

    public class EmployeeStoredInformation
    {
        public EmployeeStoredInformation(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public string Name { get; set; }

        public Sex Sex { get; set; } = Sex.Undefined;

        public string DepartmentId { get; set; }

        public byte[] Photo { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime? FireDate { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool IsWorking { get; set; }

        public string RoomNumber { get; set; }

        public string Position { get; set; }

        public string Phone { get; set; }

        public int? Age => CalculateYearsFromDate(this.BirthDate);

        public int? YearsServed => CalculateYearsFromDate(this.HireDate); //TODO: make note of FireData

        private static int? CalculateYearsFromDate(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }

            var years = DateTime.Now.Year - date.Value.Year;
            if (DateTime.Now.DayOfYear < date.Value.DayOfYear)
            {
                years = years - 1;
            }

            return years;
        }
    }
}
namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;

    public enum Sex { Male, Female, Undefined }

    public class EmployeeInfo
    {
        public EmployeeInfo(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public string Name { get; set; }

        public Sex Sex { get; set; } = Sex.Undefined;

        public string PhotoBase64Min { get; set; }

        public string DepartmentId { get; set; }

        public string PhotoBase64 { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? Age => CalculateYearsFromDate(this.BirthDate);

        public int? YearsServed => CalculateYearsFromDate(this.HireDate);

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
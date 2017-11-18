namespace Arcadia.Assistant.Organization
{
    using System;

    public enum Sex { Male, Female, Undefined }

    public class EmployeeDemographics
    {
        public EmployeeDemographics(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

        public string Name { get; set; }

        public Sex Sex { get; set; } = Sex.Undefined;

        public string PhotoBase64 { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age => CalculateYearsFromDate(this.BirthDate);

        public int YearsServed => CalculateYearsFromDate(this.HireDate);

        private static int CalculateYearsFromDate(DateTime date)
        {
            var years = DateTime.Now.Year - date.Year;
            if (DateTime.Now.DayOfYear < date.DayOfYear)
            {
                years = years - 1;
            }

            return years;
        }
    }
}
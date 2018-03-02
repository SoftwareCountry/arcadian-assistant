namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;

    public class DateQuery
    {
        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public bool Matches(DateTime date)
        {
            var daysDoNotMatch = (this.Day != null) && (this.Day != date.Day);
            var monthsDoNotMatch = (this.Month != null) && (this.Month != date.Month);
            var yearsDoNotMatch = (this.Year != null) && (this.Year != date.Year);

            return !(daysDoNotMatch || monthsDoNotMatch || yearsDoNotMatch);
        }
    }
}
namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;

    public class DateQuery
    {
        public int? FromDay { get; set; }

        public int? ToDay { get; set; }

        public int? FromMonth { get; set; }

        public int? ToMonth { get; set; }

        public int? FromYear { get; set; }

        public int? ToYear { get; set; }

        public int? Day
        {
            set
            {
                this.FromDay = value;
                this.ToDay = value;
            }
        }

        public int? Month
        {
            set
            {
                this.FromMonth = value;
                this.ToMonth = value;
            }
        }

        public int? Year
        {
            set
            {
                this.FromYear = value;
                this.ToYear = value;
            }
        }


        public bool Matches(DateTime date)
        {
            var fromDaysDoNotMatch = (this.FromDay != null) && (this.FromDay > date.Day);
            var toDaysDoNotMatch = (this.ToDay != null) && (this.ToDay < date.Day);

            var fromMonthDoNotMatch = (this.FromMonth != null) && (this.FromMonth > date.Month);
            var toMonthDoNotMatch = (this.ToMonth != null) && (this.ToMonth < date.Month);

            var fromYearDoNotMatch = (this.FromYear != null) && (this.FromYear > date.Year);
            var toYearDoNotMatch = (this.ToYear != null) && (this.ToYear < date.Year);

            var daysDoNotMatch = fromDaysDoNotMatch || toDaysDoNotMatch;
            var monthsDoNotMatch = fromMonthDoNotMatch || toMonthDoNotMatch;
            var yearsDoNotMatch = fromYearDoNotMatch || toYearDoNotMatch;

            return !(daysDoNotMatch || monthsDoNotMatch || yearsDoNotMatch);
        }
    }
}
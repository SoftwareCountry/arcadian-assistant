namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.Linq;

    public static class CalendarEventTypes
    {
        public const string Vacation = "Vacation";

        public const string Dayoff = "Dayoff";

        public const string Workout = "Workout";

        public const string Sickleave = "Sickleave";

        public static readonly string[] All = { Vacation, Dayoff, Workout, Sickleave };

        public static bool IsKnownType(string x)
        {
            return All.Contains(x, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
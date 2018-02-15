namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;
    using System.Linq;

    public static class CalendarEventStatuses
    {
        public const string Requested = "Requested";

        public const string Approved = "Approved";

        public const string Cancelled = "Cancelled";

        public static readonly string[] All = { Requested, Approved, Cancelled };

        public static bool IsKnownStatus(string x) => All.Contains(x, StringComparer.InvariantCultureIgnoreCase);
    }
}
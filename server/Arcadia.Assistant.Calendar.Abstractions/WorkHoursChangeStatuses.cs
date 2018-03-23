namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class WorkHoursChangeStatuses
    {
        public static string Requested = "Requested";

        public static string Cancelled = "Cancelled";

        public static string Approved = "Approved";

        public static string Rejected = "Rejected";

        public static readonly string[] All = { Requested, Approved, Cancelled, Rejected };
    }
}
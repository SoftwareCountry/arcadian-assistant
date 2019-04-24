namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class WorkHoursChangeStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public static readonly string[] All = { Requested, Approved, Cancelled, Rejected };

        public static readonly string[] Pending = { Requested };

        public static readonly string[] Actual = { Requested, Approved };
    }
}
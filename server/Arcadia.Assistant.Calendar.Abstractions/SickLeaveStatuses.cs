namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class SickLeaveStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Completed = "Completed";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public static readonly string[] All = { Requested, Completed, Cancelled, Approved, Rejected };
    }
}
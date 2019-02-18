namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class VacationStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public const string Processed = "Processed";

        public static readonly string[] All = { Requested, Approved, Cancelled, Rejected, Processed };

        public static readonly string[] Pending = { Requested };
    }
}
namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class VacationStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public static readonly string[] All = { Requested, Approved, Cancelled, Rejected };
    }
}
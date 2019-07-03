namespace Arcadia.Assistant.Web.Models.Calendar
{
    public static class VacationStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public const string AccountingReady = "AccountingReady";

        public const string Processed = "Processed";

        public static readonly string[] All = { Requested, Approved, Cancelled, Rejected, AccountingReady, Processed };

        public static readonly string[] Pending = { Requested };

        public static readonly string[] Actual = { Requested, Approved, AccountingReady, AccountingReady, Processed };
    }
}
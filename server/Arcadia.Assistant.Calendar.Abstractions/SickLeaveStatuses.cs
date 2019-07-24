namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class SickLeaveStatuses
    {
        public const string Requested = "Requested";

        public const string Cancelled = "Cancelled";

        public const string Completed = "Completed";

        public static readonly string[] All = { Requested, Completed, Cancelled };

        public static readonly string[] Actual = { Requested, Completed };
    }
}
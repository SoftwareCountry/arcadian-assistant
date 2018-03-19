namespace Arcadia.Assistant.Calendar.Abstractions
{
    public static class SickLeaveStatuses
    {
        public static string Requested = "Requested";

        public static string Cancelled = "Cancelled";

        public static string Completed = "Completed";

        public static readonly string[] All = { Requested, Completed, Cancelled};
    }
}
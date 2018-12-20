namespace Arcadia.Assistant.Notifications.Push
{
    using System;
    using System.Linq;

    public static class PushDeviceTypes
    {
        public static readonly string Android = "Android";

        public static readonly string Ios = "Ios";

        public static readonly string[] All = { Android, Ios };

        public static bool IsKnownType(string x) => All.Contains(x, StringComparer.InvariantCultureIgnoreCase);
    }
}
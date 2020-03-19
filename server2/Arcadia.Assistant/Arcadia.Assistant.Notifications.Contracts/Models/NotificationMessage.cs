namespace Arcadia.Assistant.Notifications.Contracts.Models
{
    using System.Collections.Generic;

    public class NotificationMessage
    {
        public string NotificationTemplate { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string LongText { get; set; } = string.Empty;

        public string ShortText { get; set; } = string.Empty;

        public IReadOnlyDictionary<string, string> Parameters { get; set; } =
            new Dictionary<string, string>();

        public static class ParameterNames
        {
            public const string DeviceType = "DeviceType";
        }
    }
}
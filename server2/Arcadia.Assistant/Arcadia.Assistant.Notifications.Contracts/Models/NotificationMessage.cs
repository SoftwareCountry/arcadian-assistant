namespace Arcadia.Assistant.Notifications.Contracts.Models
{
    public class NotificationMessage
    {
        public string ClientName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Object { get; set; } = string.Empty;

        public string LongText { get; set; } = string.Empty;

        public string ShortText { get; set; } = string.Empty;
    }
}
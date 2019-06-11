namespace Arcadia.Assistant.Web.Configuration
{
    public class UpdateNotificationSettings : IUpdateNotificationSettings
    {
        public string NotificationTitle { get; set; }

        public string NotificationBody { get; set; }
    }
}
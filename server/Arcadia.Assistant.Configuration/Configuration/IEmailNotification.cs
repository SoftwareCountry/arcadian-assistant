namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IEmailNotification
    {
        string NotificationSender { get; }

        string Subject { get; }

        string Body { get; }
    }
}
namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IEmailSettings
    {
        string NotificationSender { get; }

        string Subject { get; }

        string Body { get; }
    }
}
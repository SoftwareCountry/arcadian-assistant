namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IEmailSettings
    {
        string NotificationRecipient { get; }

        string NotificationSender { get; }

        string Subject { get; }

        string Body { get; }
    }
}

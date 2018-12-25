namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IEmailWithFixedRecipientNotification : IEmailNotification
    {
        string NotificationRecipient { get; }
    }
}
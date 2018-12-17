namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IEmailWithFixedRecipientSettings : IEmailSettings
    {
        string NotificationRecipient { get; }
    }
}
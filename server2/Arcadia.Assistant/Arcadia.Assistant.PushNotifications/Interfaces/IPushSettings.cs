namespace Arcadia.Assistant.PushNotifications.Interfaces
{
    public interface IPushSettings
    {
        string ApiToken { get; }

        string AndroidPushUrl { get; }

        string IosPushUrl { get; }
    }
}
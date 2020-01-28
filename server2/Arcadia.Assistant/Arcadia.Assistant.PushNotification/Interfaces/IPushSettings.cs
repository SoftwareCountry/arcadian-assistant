namespace Arcadia.Assistant.PushNotification.Interfaces
{
    public interface IPushSettings
    {
        bool Enabled { get; }

        string ApiToken { get; }

        string AndroidPushUrl { get; }

        string IosPushUrl { get; }
    }
}
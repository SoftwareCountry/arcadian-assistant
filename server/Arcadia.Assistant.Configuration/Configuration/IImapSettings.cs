namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface IImapSettings
    {
        bool Enabled { get; }

        string User { get; }

        string Password { get; }

        int Port { get; }

        string Host { get; }

        int RefreshIntervalMinutes { get; }
    }
}

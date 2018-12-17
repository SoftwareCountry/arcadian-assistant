namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ISmtpSettings
    {
        bool Enabled { get; }

        string User { get; }

        string Password { get; }

        int Port { get; }

        string Host { get; }

        bool UseTls { get; }
    }
}

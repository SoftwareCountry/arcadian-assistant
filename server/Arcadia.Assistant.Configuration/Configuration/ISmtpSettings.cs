namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ISmtpSettings
    {
        string User { get; }
        string Password { get; }
        int Port { get; }
        string Host { get; }
    }
}

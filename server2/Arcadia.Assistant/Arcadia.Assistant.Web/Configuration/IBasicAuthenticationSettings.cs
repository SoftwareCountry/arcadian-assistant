namespace Arcadia.Assistant.Web.Configuration
{
    public interface IBasicAuthenticationSettings
    {
        string? Realm { get; }

        string? Login { get; }

        string? Password { get; }
    }
}
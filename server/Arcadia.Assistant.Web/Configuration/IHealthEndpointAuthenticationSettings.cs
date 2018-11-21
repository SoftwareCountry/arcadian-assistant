namespace Arcadia.Assistant.Web.Configuration
{
    public interface IHealthEndpointAuthenticationSettings
    {
        string Realm { get; }

        string Login { get; }

        string Password { get; }
    }
}
namespace Arcadia.Assistant.Web.Configuration
{
    public interface IServiceEndpointsAuthenticationSettings
    {
        string Realm { get; }

        string Login { get; }

        string Password { get; }
    }
}
namespace Arcadia.Assistant.Web.Configuration
{
    public interface ISecuritySettings
    {
        string ClientId { get; }

        string AuthorizationUrl { get; }

        string TokenUrl { get; }

        string SwaggerRedirectUri { get; }

        string OpenIdConfigurationUrl { get; }
    }
}
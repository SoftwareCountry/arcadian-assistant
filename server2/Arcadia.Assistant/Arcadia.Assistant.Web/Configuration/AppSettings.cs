namespace Arcadia.Assistant.Web.Configuration
{
    using Logging;

    public class AppSettings
    {
        public ConfigPackageSettings Config { get; set; } = new ConfigPackageSettings();

        public class ConfigPackageSettings
        {
            public OpenIdSettings OpenId { get; set; } = new OpenIdSettings();

            public LinkConfiguration Links { get; set; } = new LinkConfiguration();

            public SslConfiguration Ssl { get; set; } = new SslConfiguration();

            public BasicAuthenticationSettings BasicAuthentication { get; set; } = new BasicAuthenticationSettings();

            public LoggerSettings Logging { get; set; } = new LoggerSettings();
        }
    }
}
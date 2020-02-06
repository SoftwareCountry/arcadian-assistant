namespace Arcadia.Assistant.Web.Configuration
{
    using Logging;

    public class AppSettings
    {
        public ConfigPackageSettings Config { get; set; } = new ConfigPackageSettings();

        public class ConfigPackageSettings
        {
            public SecuritySettings Security { get; set; } = new SecuritySettings();

            public LinkConfiguration Links { get; set; } = new LinkConfiguration();

            public SslConfiguration Ssl { get; set; } = new SslConfiguration();

            public LoggerSettings Logging { get; set; } = new LoggerSettings();
        }
    }
}
using Arcadia.Assistant.AppCenterBuilds.Contracts;

namespace Arcadia.Assistant.Web.Configuration
{
    public class AppSettings
    {
        public ConfigPackageSettings Config { get; set; } = new ConfigPackageSettings();

        public class LinkConfiguration : IHelpSettings
        {
            public string? HelpLink { get; set; }
        }

        public class SslConfiguration : ISslSettings
        {
            public bool? SslOffloading { get; set; }
        }

        public class ConfigPackageSettings
        {
            public SecuritySettings Security { get; set; } = new SecuritySettings();

            public LinkConfiguration Links { get; set; } = new LinkConfiguration();

            public SslConfiguration Ssl { get; set; } = new SslConfiguration();
        }
    }
}
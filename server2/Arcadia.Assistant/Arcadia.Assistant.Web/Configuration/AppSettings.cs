namespace Arcadia.Assistant.Web.Configuration
{
    public class AppSettings
    {
        public ConfigPackageSettings Config { get; set; }

        public class ConfigPackageSettings
        {
            public SecuritySettings Security { get; set; }
        }
    }
}
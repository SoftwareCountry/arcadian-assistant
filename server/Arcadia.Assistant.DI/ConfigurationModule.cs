namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP;

    using Autofac;

    using Microsoft.Extensions.Configuration;

    public class ConfigurationModule : Module
    {
        private readonly IConfigurationRoot configurationRoot;

        public ConfigurationModule(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = this.configurationRoot.Get<AppSettings>();
            builder.RegisterInstance(settings).AsSelf();
            builder.RegisterInstance(settings.Organization.RefreshInformation).As<IRefreshInformation>();

            var cspSettings = this.configurationRoot.GetSection("Csp").Get<CspConfiguration>();
            builder.RegisterInstance(cspSettings).AsSelf();
        }
    }
}
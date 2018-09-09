namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.CSP.Vacations;

    using Autofac;

    using Microsoft.Extensions.Configuration;

    public class Remote1CModule : Module
    {
        private readonly IConfigurationRoot configuration;

        public Remote1CModule(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = this.configuration.GetSection("Remote1C").Get<Remote1CConfiguration>();
            builder.RegisterInstance(settings).AsSelf();
        }
    }
}
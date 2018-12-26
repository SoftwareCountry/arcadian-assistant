namespace Arcadia.Assistant.DI
{
    using Microsoft.Extensions.Configuration;

    using Autofac;

    using Arcadia.Assistant.CSP.Vacations;

    public class ArcadiaCspModule : Module
    {
        private readonly IConfigurationRoot configuration;

        public ArcadiaCspModule(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = this.configuration
                .GetSection("VacationsEmailLoader")
                .Get<VacationsEmailLoaderConfiguration>();
            builder.RegisterInstance(settings).AsSelf();
        }
    }
}
namespace Arcadia.Assistant.Server
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Arcadia.Assistant.DI;
    using Arcadia.Assistant.Configuration.Configuration;

    public class DependencyInjection
    {
        public IContainer GetContainer(IConfigurationRoot config, ServiceCollection serviceCollection)
        {
            var settings = config.Get<AppSettings>();
            var container = new ContainerBuilder();

            container.Populate(serviceCollection);

            container.RegisterModule(new ConfigurationModule(config));

            container.RegisterModule(new DatabaseModule(config["ConnectionStrings:ArcadiaCSP"]));

            var organizationSettings = settings.Organization;
            container.RegisterModule(new OrganizationModule(organizationSettings.RefreshInformation));

            var messagingSettings = settings.Messaging;
            container.RegisterModule(new NotificationsModule(messagingSettings.Smtp, messagingSettings.Push));

            container.RegisterModule(new Remote1CModule(config));
            container.RegisterModule(new HealthModule());

            return container.Build();
        }
    }
}
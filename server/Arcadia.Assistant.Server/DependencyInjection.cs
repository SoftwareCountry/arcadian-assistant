namespace Arcadia.Assistant.Server
{
    using Akka.Actor;
    using Arcadia.Assistant.DI;

    using Autofac;
    using Configuration.Configuration;
    using Microsoft.Extensions.Configuration;

    public class DependencyInjection
    {
        public IContainer GetContainer(IConfigurationRoot config)
        {
            var settings = config.Get<AppSettings>();
            var container = new ContainerBuilder();

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
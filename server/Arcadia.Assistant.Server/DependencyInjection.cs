namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.DI;

    using Autofac;
    using Configuration.Configuration;
    using Microsoft.Extensions.Configuration;

    public class DependencyInjection
    {
        public IContainer GetContainer(IConfigurationRoot config)
        {
            var container = new ContainerBuilder();

            container.RegisterModule(new DatabaseModule(config.GetConnectionString("ArcadiaCSP")));

            var organizationSettings = config.Get<AppSettings>().Organization;
            container.RegisterModule(new OrganizationModule(organizationSettings.RefreshInformation));

            var mailSettings = config.Get<AppSettings>().Messaging;
            container.RegisterModule(new NotificationsModule(mailSettings.Smtp, mailSettings.SickLeave));

            return container.Build();
        }
    }
}
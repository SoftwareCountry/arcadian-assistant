namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.DI;

    using Autofac;
    using Configuration.Configuration;
    using Microsoft.Extensions.Configuration;

    public class DependencyInjection
    {
        public IContainer GetContainer(AppSettings config)
        {
            var container = new ContainerBuilder();

            container.RegisterModule(new DatabaseModule(config.ConnectionStrings.ArcadiaCSP));

            var organizationSettings = config.Organization;
            container.RegisterModule(new OrganizationModule(organizationSettings.RefreshInformation));

            var mailSettings = config.Messaging;
            container.RegisterModule(new NotificationsModule(mailSettings.Smtp, mailSettings.SickLeave));

            return container.Build();
        }
    }
}
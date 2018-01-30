namespace Arcadia.Assistant.Server
{
    using Arcadia.Assistant.DI;

    using Autofac;

    using Microsoft.Extensions.Configuration;

    public class DependencyInjection
    {
        public IContainer GetContainer(IConfigurationRoot config)
        {
            var container = new ContainerBuilder();

            container.RegisterModule(new DatabaseModule(config.GetConnectionString("ArcadiaCSP")));
            container.RegisterModule<OrganizationModule>();

            return container.Build();
        }
    }
}
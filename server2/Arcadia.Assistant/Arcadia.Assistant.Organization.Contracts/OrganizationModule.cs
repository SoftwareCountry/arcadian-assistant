namespace Arcadia.Assistant.Organization.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

    public class OrganizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IOrganization>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Organization"), new ServicePartitionKey(0)));
        }
    }
}
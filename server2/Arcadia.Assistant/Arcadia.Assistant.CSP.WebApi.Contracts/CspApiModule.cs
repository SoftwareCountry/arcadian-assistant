namespace Arcadia.Assistant.CSP.WebApi.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class CspApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<ICspApi>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.CSP.WebApi"),
                    new ServicePartitionKey(0)));
        }
    }
}
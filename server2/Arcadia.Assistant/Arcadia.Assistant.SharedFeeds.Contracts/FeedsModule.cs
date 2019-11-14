namespace Arcadia.Assistant.SharedFeeds.Contracts
{
    using System;
    using Autofac;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class FeedsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IFeeds>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.SharedFeedsType"), new ServicePartitionKey(0)));
        }
    }
}
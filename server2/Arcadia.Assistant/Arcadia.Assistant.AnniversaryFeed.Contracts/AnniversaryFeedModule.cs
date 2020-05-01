namespace Arcadia.Assistant.AnniversaryFeed.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class AnniversaryFeedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IAnniversaryFeed>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.AnniversaryFeed")));
        }
    }
}
namespace Arcadia.Assistant.BirthdaysFeed.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class BirthdaysFeedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IBirthdaysFeed>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.BirthdaysFeed")));
        }
    }
}
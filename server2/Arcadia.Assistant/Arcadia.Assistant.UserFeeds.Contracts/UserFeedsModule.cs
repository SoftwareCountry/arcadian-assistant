namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;

    using Autofac;

    using Interfaces;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class UserFeedsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IUserFeeds>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.UserFeeds")));
        }
    }
}
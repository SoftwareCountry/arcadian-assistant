namespace Arcadia.Assistant.Inbox.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class InboxModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IInbox>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Inbox")));
        }
    }
}
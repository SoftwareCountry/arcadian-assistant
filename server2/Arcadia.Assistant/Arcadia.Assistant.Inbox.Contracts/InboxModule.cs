namespace Arcadia.Assistant.Inbox.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

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
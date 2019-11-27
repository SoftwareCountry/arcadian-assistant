namespace Arcadia.Assistant.PendingActions.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

    public class PendingActionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IPendingActions>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.PendingActions")));
        }
    }
}
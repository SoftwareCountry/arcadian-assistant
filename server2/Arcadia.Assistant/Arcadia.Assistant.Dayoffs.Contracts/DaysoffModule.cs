namespace Arcadia.Assistant.Dayoffs.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class DaysoffModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IDayoffsService>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Dayoffs")));
        }
    }
}
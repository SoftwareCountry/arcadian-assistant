namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class SickLeavesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<ISickLeaves>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.SickLeaves")));
        }
    }
}
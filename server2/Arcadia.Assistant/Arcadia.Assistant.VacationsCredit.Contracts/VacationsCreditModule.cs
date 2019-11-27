namespace Arcadia.Assistant.VacationsCredit.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

    public class VacationsCreditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IVacationsCredit>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.VacationsCredit")));
        }
    }
}
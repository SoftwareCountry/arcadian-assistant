namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class VacationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IVacations>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Vacations")));
        }
    }
}
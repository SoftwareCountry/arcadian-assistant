namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class WorkHoursCreditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IWorkHoursCredit>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.WorkHoursCredit")));
        }
    }
}
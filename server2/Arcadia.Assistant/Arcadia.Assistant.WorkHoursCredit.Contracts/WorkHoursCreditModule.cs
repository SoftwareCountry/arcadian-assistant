﻿namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

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
﻿namespace Arcadia.Assistant.Employees.Contracts
{
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System;

    public class EmployeesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IEmployees>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Employees")));
        }
    }
}
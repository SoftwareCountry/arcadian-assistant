﻿namespace Arcadia.Assistant.DeviceRegistry.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class DeviceRegistryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IDeviceRegistry>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.DeviceRegistry")));
        }
    }
}
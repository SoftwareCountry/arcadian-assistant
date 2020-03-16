namespace Arcadia.Assistant.Permissions.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class PermissionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IPermissions>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Permissions")));

            builder.RegisterType<PermissionsEntryQuery>().AsSelf();
        }
    }
}
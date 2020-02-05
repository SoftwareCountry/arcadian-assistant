namespace Arcadia.Assistant.PushNotifications.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class PushNotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IPushNotifications>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.PushNotifications")));
        }
    }
}
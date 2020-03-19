namespace Arcadia.Assistant.Notifications.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class NotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<INotifications>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Notifications")));
        }
    }
}
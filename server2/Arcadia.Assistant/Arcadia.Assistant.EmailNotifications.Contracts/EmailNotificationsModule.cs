namespace Arcadia.Assistant.EmailNotifications.Contracts
{
    using System;

    using Autofac;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class EmailNotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IEmailNotifications>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.EmailNotifications")));
        }
    }
}
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.Notifications
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using DeviceRegistry.Contracts;

    using Interfaces;

    using Microsoft.Extensions.DependencyInjection;

    using Models;

    using PushNotifications.Contracts;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.Register(x => new NotificationSettings(configurationPackage.Settings.Sections["Notifications"])).As<INotificationSettings>().SingleInstance();
                builder.RegisterStatelessService<Notifications>("Arcadia.Assistant.NotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule<DeviceRegistryModule>();
                builder.RegisterModule<PushNotificationsModule>();
                builder.Populate(services);

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}

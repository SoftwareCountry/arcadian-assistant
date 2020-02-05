using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.PushNotifications.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.PushNotifications
{
    using Arcadia.Assistant.PushNotifications.Models;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Microsoft.Extensions.DependencyInjection;

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
                builder.Register(x => new PushSettings(configurationPackage.Settings.Sections["PushNotifications"])).As<IPushSettings>().SingleInstance();
                builder.RegisterStatelessService<PushNotifications>("Arcadia.Assistant.PushNotificationsType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
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

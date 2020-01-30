namespace Arcadia.Assistant.PushNotificationsDistributor
{
    using System;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Contracts;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Actors.Client;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
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
                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterModule(new PushNotificationsDistributionModule());
                builder.RegisterActor<PushNotificationsDistributionActor>();
                builder.Populate(services);

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
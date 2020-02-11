namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Logging;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using MobileBuild.Contracts;

    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            ILogger? logger = null;
            try
            {
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<AppCenterBuilds>("Arcadia.Assistant.AppCenterBuildsType");
                builder.Register(x => new DownloadApplicationSettings(configurationPackage.Settings.Sections["DownloadApplication"])).As<IDownloadApplicationSettings>().SingleInstance();

                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new MobileBuildModule());
                builder.Populate(services);

                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using var container = builder.Build();
                logger = container.TryResolve<ILogger>(out ILogger val) ? val : null;
                logger?.LogInformation($"Service type '{typeof(AppCenterBuilds).Name}' registered. Process: {Process.GetCurrentProcess().Id}.");
                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                logger?.LogCritical(e, e.Message);
                throw;
            }
        }
    }
}
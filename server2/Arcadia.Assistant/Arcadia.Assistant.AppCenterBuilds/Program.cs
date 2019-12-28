namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Core.Registration;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Logging;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Microsoft.ServiceFabric.Services.Runtime;

    using MobileBuild.Contracts;

    using Serilog;

    using ServiceFabric.Logging;

    using ILogger = Microsoft.Extensions.Logging.ILogger;

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
                builder.RegisterStatelessService<AppCenterBuilds>("Arcadia.Assistant.AppCenterBuildsType");
                builder.Register(x => new DownloadApplicationSettings(configurationPackage.Settings.Sections["DownloadApplication"])).As<IDownloadApplicationSettings>().SingleInstance();

                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new MobileBuildModule());
                builder.Populate(services);

                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));

                using (builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(AppCenterBuilds).Name);

                    // Prevents this host process from terminating so services keep running.
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
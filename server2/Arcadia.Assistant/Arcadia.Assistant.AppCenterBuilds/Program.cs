using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Arcadia.Assistant.AppCenterBuilds.Contracts;
using Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces;
using Arcadia.Assistant.MobileBuild.Contracts;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.ServiceFabric;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Actors.Client;

namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Integration.ServiceFabric;

    using Contracts;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Actors.Client;
using Arcadia.Assistant.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;

    using MobileBuild.Contracts;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            ILogger logger = null;
            try
            {
                var configurationPackage = FabricRuntime.GetActivationContext().GetConfigurationPackageObject("Config");
                var appInsightKey = configurationPackage.Settings.Sections["Logging"].Parameters["ApplicationInsightsKey"].Value;

                var services = new ServiceCollection();
                services.AddHttpClient();

                var builder = new ContainerBuilder();
                builder.RegisterServiceFabricSupport();
                builder.Register(x => new DownloadApplicationSettings(configurationPackage.Settings.Sections["DownloadApplication"])).As<IDownloadApplicationSettings>().SingleInstance();
                builder.Register(x => new LoggerSettings(appInsightKey)).SingleInstance();
                builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterModule(new MobileBuildModule());
                builder.RegisterStatelessService<AppCenterBuilds>("Arcadia.Assistant.AppCenterBuildsType");
                builder.Populate(services);

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

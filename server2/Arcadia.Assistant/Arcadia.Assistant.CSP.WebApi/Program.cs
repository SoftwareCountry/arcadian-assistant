using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.CSP.WebApi.Contracts;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.CSP.WebApi
{
    using Autofac.Extensions.DependencyInjection;

    using Logging;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
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
                builder.Register(x => new CspConfiguration
                {
                    ConnectionString =
                        "http://arcadiacspdb00v:8080/api/",
                    //"Endpoint=sb://arcadia-cspdb-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=My3KYmJOIIgqo9fBP9DYxbqiNfIsAKI7AlrZWfQz38I=",
                    CompanyId = 154, //TODO: config file
                    HeadDepartmentAbbreviation = "GMG",
                    UserIdentityDomain = "arcadia.spb.ru"
                }).As<CspConfiguration>().SingleInstance();
                builder.RegisterStatelessService<WebApi>("Arcadia.Assistant.CSP.WebApiType");
                builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
                builder.RegisterServiceLogging(new LoggerSettings(configurationPackage.Settings.Sections["Logging"]));
                builder.Populate(services);

                using var container = builder.Build();
                logger = container.ResolveOptional<ILogger<WebApi>>();
                logger?.LogInformation("Service type '{ServiceName}' registered. Process: {ProcessId}.",
                    typeof(WebApi).Name, Process.GetCurrentProcess().Id);
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

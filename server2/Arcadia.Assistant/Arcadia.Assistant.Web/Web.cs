namespace Arcadia.Assistant.Web
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using Arcadia.Assistant.Logging;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using ServiceFabric.Logging;

    /// <summary>
    ///     The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    public class Web : StatelessService
    {
        private readonly ILogger logger;
        public Web(StatelessServiceContext context, ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        /// <summary>
        ///     Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");
                        return new WebHostBuilder()
                            .UseKestrel()
                            .ConfigureServices(
                                services => services
                                    .AddSingleton(serviceContext)
                                    .AddAutofac()
                                    .AddSingleton(this.logger))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .ConfigureLogging(logger =>
                            {
                                logger.ClearProviders();
                            })
                            .UseStartup<Startup>()
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    }))
            };
        }
    }
}
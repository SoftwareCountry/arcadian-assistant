namespace Arcadia.Assistant.Web
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using Arcadia.Assistant.Logging;
    using Autofac.Extensions.DependencyInjection;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Web : StatelessService
    {
        private readonly string appInsightsKey;
        private readonly ILogger logger;

        public Web(StatelessServiceContext context, LoggerSettings settings)
            : base(context)
        {

            var loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory(settings.ApplicationInsightsKey);
            logger = loggerFactory.CreateLogger<Web>();
            appInsightsKey = settings.ApplicationInsightsKey;
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
                        //ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");
                        logger?.LogInformation($"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                            .UseKestrel()
                            .ConfigureServices(
                                services => services
                                    .AddSingleton(serviceContext)
                                    .AddSingleton(logger))
                            .ConfigureServices(services => services.AddAutofac())
                            .ConfigureLogging((hosingContext, logging) =>
                            {
                                logging.AddConfiguration(hosingContext.Configuration.GetSection("Logging"));
                                logging.AddConsole();
                                logging.AddDebug();
                                logging.AddEventSourceLogger();
                            })
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    }))
            };
        }
    }
}
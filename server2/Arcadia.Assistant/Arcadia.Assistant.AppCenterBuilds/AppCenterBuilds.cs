using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.AppCenterBuilds.Contracts;
using Arcadia.Assistant.AppCenterBuilds.Contracts.AppCenter;
using Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces;
using Arcadia.Assistant.Logging;
using Arcadia.Assistant.MobileBuild.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Arcadia.Assistant.AppCenterBuilds
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AppCenterBuilds : StatelessService
    {
        private readonly IDownloadApplicationSettings configuration;
        private readonly IHttpClientFactory clientFactory;
        private readonly IMobileBuildActorFactory mobileBuildFactory;
        private readonly ILogger logger;

        public AppCenterBuilds(StatelessServiceContext context, IDownloadApplicationSettings configuration, IHttpClientFactory clientFactory, IMobileBuildActorFactory mobileBuildFactory, LoggerSettings loggerSettings)
            : base(context)
        {
            this.configuration = configuration;
            this.clientFactory = clientFactory;
            this.mobileBuildFactory = mobileBuildFactory;
            var loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory(loggerSettings?.ApplicationInsightsKey);
            this.logger = loggerFactory.CreateLogger<AppCenterBuilds>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //ServiceEventSource.Current.ServiceMessage(this.Context, "Request build version");
                logger?.LogDebug("Request build version");

                if (this.configuration != null)
                {
                    // for android
                    var result = await UpdateMobileBuild(ApplicationTypeEnum.Android, this.configuration?.AndroidGetBuildsUrl ?? string.Empty, this.configuration?.AndroidGetBuildDownloadLinkTemplateUrl ?? string.Empty, cancellationToken);
                    //ServiceEventSource.Current.ServiceMessage(this.Context, "Android build version update result: {0}", result);
                    logger?.LogInformation($"Android build version update result: {result}");

                    // for ios
                    result = await UpdateMobileBuild(ApplicationTypeEnum.Ios, this.configuration?.IosGetBuildsUrl ?? string.Empty, this.configuration?.IosGetBuildDownloadLinkTemplateUrl ?? string.Empty, cancellationToken);
                    //ServiceEventSource.Current.ServiceMessage(this.Context, "Ios build version update result: {0}", result);
                    logger?.LogInformation($"Ios build version update result: {result}");
                }
                else
                {
                    //ServiceEventSource.Current.ServiceMessage(this.Context, "Configuration is empty");
                    logger?.LogInformation("Configuration is empty");
                }

                await Task.Delay(TimeSpan.FromMinutes(this.DownloadBuildIntervalMinutes), cancellationToken);
            }
        }

        private int DownloadBuildIntervalMinutes { get => this.configuration == null ? 720 : this.configuration.DownloadBuildIntervalMinutes; }

        private string ApiToken { get => this.configuration?.ApiToken ?? string.Empty; }

        private async Task<bool> UpdateMobileBuild(ApplicationTypeEnum type, string buildUrl, string buildDownloadUrlTemplate, CancellationToken cancellationToken)
        {
            try
            {
                var logStore = new Action<string>(x => /*ServiceEventSource.Current.ServiceMessage(this.Context, x)*/logger?.LogInformation(x));
                var mobileType = type.ToString();
                var actor = mobileBuildFactory.MobileBuild(mobileType);
                var updateHelper = new UpdateMobileBuildHelper(buildUrl, buildDownloadUrlTemplate, ApiToken);
                return await updateHelper.CheckAndupdateMobileBuild(this.clientFactory, actor, cancellationToken, logStore);
            }
            catch(Exception ex)
            {
                //ServiceEventSource.Current.ServiceMessage(this.Context, "{0} mobile type version udpate exception: {1}", type, ex.Message);
                logger?.LogError(ex, "{0} mobile type version udpate exception: {1}", type, ex.Message);
                return false;
            }
        }
    }
}

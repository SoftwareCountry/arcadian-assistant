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
using Arcadia.Assistant.MobileBuild.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using MobileBuild.Contracts;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AppCenterBuilds : StatelessService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IDownloadApplicationSettings configuration;
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

        private int DownloadBuildIntervalMinutes => this.configuration.DownloadBuildIntervalMinutes;

        private string ApiToken // null - rise the exception
            =>
                this.configuration.ApiToken;

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

                    // for android
                await this.UpdateMobileBuild(ApplicationType.Android, this.configuration.AndroidGetBuildsUrl, this.configuration.AndroidGetBuildDownloadLinkTemplateUrl, cancellationToken);

                    // for ios
                await this.UpdateMobileBuild(ApplicationType.Ios, this.configuration.IosGetBuildsUrl, this.configuration.IosGetBuildDownloadLinkTemplateUrl, cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(this.DownloadBuildIntervalMinutes), cancellationToken);
            }
        }

        private async Task UpdateMobileBuild(ApplicationType type, string buildUrl, string buildDownloadUrlTemplate, CancellationToken cancellationToken)
        {
            try
            {
                var logStore = new Action<string>(x => /*ServiceEventSource.Current.ServiceMessage(this.Context, x)*/logger?.LogInformation(x));
                var mobileType = type.ToString();
                var actor = this.mobileBuildFactory.MobileBuild(mobileType);
                var updateHelper = new UpdateMobileBuildHelper(buildUrl, buildDownloadUrlTemplate, this.ApiToken);
                await updateHelper.CheckAndUpdateMobileBuild(this.clientFactory, actor, cancellationToken, logStore);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{0} mobile type version udpate exception: {1}", type, ex.Message);
            }
        }
    }
}

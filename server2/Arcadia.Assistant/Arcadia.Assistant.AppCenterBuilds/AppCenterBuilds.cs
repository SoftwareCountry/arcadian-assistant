namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using MobileBuild.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class AppCenterBuilds : StatelessService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IDownloadApplicationSettings configuration;
        private readonly ILogger logger;
        private readonly IMobileBuildActorFactory mobileBuildFactory;

        public AppCenterBuilds(StatelessServiceContext context, IDownloadApplicationSettings configuration, IHttpClientFactory clientFactory, IMobileBuildActorFactory mobileBuildFactory, ILogger<AppCenterBuilds> logger)
            : base(context)
        {
            this.configuration = configuration;
            this.clientFactory = clientFactory;
            this.mobileBuildFactory = mobileBuildFactory;
            this.logger = logger;
        }

        private int DownloadBuildIntervalMinutes => this.configuration.DownloadBuildIntervalMinutes;

        private string ApiToken // null - rise the exception
            =>
                this.configuration.ApiToken;

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //ServiceEventSource.Current.ServiceMessage(this.Context, "Request build version");
                this.logger.LogDebug("Request build version");

                // for android
                await this.UpdateMobileBuild(WellKnownBuildTypes.Android, this.configuration.AndroidGetBuildsUrl, this.configuration.AndroidGetBuildDownloadLinkTemplateUrl, cancellationToken);

                // for ios
                await this.UpdateMobileBuild(WellKnownBuildTypes.Ios, this.configuration.IosGetBuildsUrl, this.configuration.IosGetBuildDownloadLinkTemplateUrl, cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(this.DownloadBuildIntervalMinutes), cancellationToken);
            }
        }

        private async Task UpdateMobileBuild(string mobileType, string buildUrl, string buildDownloadUrlTemplate, CancellationToken cancellationToken)
        {
            try
            {
                var actor = this.mobileBuildFactory.MobileBuild(mobileType);
                var updateHelper = new UpdateMobileBuildHelper(buildUrl, buildDownloadUrlTemplate, this.ApiToken);
                await updateHelper.CheckAndUpdateMobileBuild(this.clientFactory, actor, cancellationToken, this.logger);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "{0} mobile type version udpate exception: {1}", mobileType, ex.Message);
            }
        }
    }
}
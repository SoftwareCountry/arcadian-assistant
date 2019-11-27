using Arcadia.Assistant.AppCenterBuilds.Contracts;
using Arcadia.Assistant.MobileBuild.Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

        private const int DefaultDownloadBuildIntervalMinutes = 720;

        public AppCenterBuilds(StatelessServiceContext context, IDownloadApplicationSettings configuration, IHttpClientFactory clientFactory, IMobileBuildActorFactory mobileBuildFactory)
            : base(context)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("Configuration is empty");
            }

            this.configuration = configuration;
            this.clientFactory = clientFactory;
            this.mobileBuildFactory = mobileBuildFactory;
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

                ServiceEventSource.Current.ServiceMessage(this.Context, "Request build version");

                // for android
                var result = await UpdateMobileBuild(ApplicationType.Android, this.configuration.AndroidGetBuildsUrl, this.configuration.AndroidGetBuildDownloadLinkTemplateUrl, cancellationToken);
                ServiceEventSource.Current.ServiceMessage(this.Context, "Android build version update result: {0}", result);

                // for ios
                result = await UpdateMobileBuild(ApplicationType.Ios, this.configuration.IosGetBuildsUrl, this.configuration.IosGetBuildDownloadLinkTemplateUrl, cancellationToken);
                ServiceEventSource.Current.ServiceMessage(this.Context, "Ios build version update result: {0}", result);

                await Task.Delay(TimeSpan.FromMinutes(this.DownloadBuildIntervalMinutes), cancellationToken);
            }
        }

        private int DownloadBuildIntervalMinutes { get => this.configuration?.DownloadBuildIntervalMinutes ?? DefaultDownloadBuildIntervalMinutes; }

        private string? ApiToken { get => this.configuration?.ApiToken; } // null - rise the exception

        private async Task<bool> UpdateMobileBuild(ApplicationType type, string? buildUrl, string? buildDownloadUrlTemplate, CancellationToken cancellationToken)
        {
            try
            {
                var logStore = new Action<string>(x => ServiceEventSource.Current.ServiceMessage(this.Context, x));
                var mobileType = type.ToString();
                var actor = mobileBuildFactory.MobileBuild(mobileType);
                var updateHelper = new UpdateMobileBuildHelper(buildUrl, buildDownloadUrlTemplate, ApiToken);
                return await updateHelper.CheckAndupdateMobileBuild(this.clientFactory, actor, cancellationToken, logStore);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "{0} mobile type version udpate exception: {1}", type, ex.Message);
                return false;
            }
        }
    }
}

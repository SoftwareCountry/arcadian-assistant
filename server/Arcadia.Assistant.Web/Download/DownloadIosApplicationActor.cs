namespace Arcadia.Assistant.Web.Download
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadIosApplicationActor : DownloadApplicationActorBase, ILogReceive
    {
        private readonly string getBuildsUrl;
        private readonly AppCenterDownloader appCenterDownloader;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private AppCenterDownloadResult latestBuildResult;

        public DownloadIosApplicationActor(
            IDownloadApplicationSettings downloadApplicationSettings,
            IHttpClientFactory httpClientFactory,
            IHostingEnvironment hostingEnvironment,
            string getBuildsUrl,
            string getBuildDownloadLinkTemplateUrl)
        {
            this.getBuildsUrl = getBuildsUrl;

            this.appCenterDownloader = new AppCenterDownloader(
                downloadApplicationSettings,
                httpClientFactory,
                hostingEnvironment,
                getBuildsUrl,
                getBuildDownloadLinkTemplateUrl);
        }

        protected override void RespondLatestBuildPath()
        {
            this.Sender.Tell(new GetLatestApplicationBuildPath.Response(this.latestBuildResult?.FilePath));
        }

        protected override void RespondDownloadApplicationBuild()
        {
            this.DownloadBuild()
                .PipeTo(
                    this.Sender,
                    success: () => DownloadApplicationBuild.Success.Instance,
                    failure: err => new DownloadApplicationBuild.Error(err));
        }

        private async Task DownloadBuild()
        {
            var newBuildResult = await this.appCenterDownloader.Download();
            if (newBuildResult == null)
            {
                this.logger.Warning($"No builds found for url {this.getBuildsUrl}");
                return;
            }

            if (this.latestBuildResult?.BuildNumber != newBuildResult.BuildNumber)
            {
                this.latestBuildResult = newBuildResult;
            }
        }
    }
}
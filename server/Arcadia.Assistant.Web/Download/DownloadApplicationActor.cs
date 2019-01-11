namespace Arcadia.Assistant.Web.Download
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadApplicationActor : UntypedActor, ILogReceive
    {
        private readonly string getBuildsUrl;
        private readonly AppCenterDownloader appCenterDownloader;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private AppCenterDownloadResult latestBuildResult;

        public DownloadApplicationActor(
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

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetLatestApplicationBuildPath _:
                    this.RespondLatestBuildPath();
                    break;

                case DownloadApplicationBuild _:
                    this.DownloadBuild()
                        .PipeTo(
                            this.Sender,
                            success: () => DownloadApplicationBuild.Success.Instance,
                            failure: err => new DownloadApplicationBuild.Error(err));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RespondLatestBuildPath()
        {
            this.Sender.Tell(new GetLatestApplicationBuildPath.Response(this.latestBuildResult?.FilePath));
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
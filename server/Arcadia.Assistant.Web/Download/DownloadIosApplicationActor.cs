namespace Arcadia.Assistant.Web.Download
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadIosApplicationActor : DownloadApplicationActorBase, ILogReceive
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;
        private readonly IHostingEnvironment hostingEnvironment;
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
            this.downloadApplicationSettings = downloadApplicationSettings;
            this.hostingEnvironment = hostingEnvironment;
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

            if (this.latestBuildResult?.BuildNumber == newBuildResult.BuildNumber)
            {
                return;
            }

            var manifestFilePath = await this.CreateManifestFile(newBuildResult.FilePath);
            this.latestBuildResult = new AppCenterDownloadResult(newBuildResult.BuildNumber, manifestFilePath);
        }

        private async Task<string> CreateManifestFile(string applicationPath)
        {
            var manifestFilePath = Path.ChangeExtension(applicationPath, ".plist");

            string manifestTemplate;

            using (var manifestTemplateFile = File.OpenText(this.downloadApplicationSettings.IosManifestTemplateFileName))
            {
                manifestTemplate = await manifestTemplateFile.ReadToEndAsync();
            }

            var relativeFilePath = Path
                .GetRelativePath(this.hostingEnvironment.ContentRootPath, applicationPath)
                .Replace("\\", "/");
            var downloadUrl = this.downloadApplicationSettings.DownloadFileUrl
                .Replace("{filePath}", relativeFilePath);
            manifestTemplate = manifestTemplate.Replace("{downloadApplicationUrl}", downloadUrl);

            using (var file = File.CreateText(manifestFilePath))
            {
                await file.WriteAsync(manifestTemplate);
            }

            return manifestFilePath;
        }
    }
}
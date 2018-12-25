namespace Arcadia.Assistant.Web.Download
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download.AppCenter;

    public class DownloadApplicationActor : UntypedActor, ILogReceive
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly string getBuildsUrl;
        private readonly string getBuildDownloadLinkTemplateUrl;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private int latestBuildNumber;
        private string latestBuildPath;

        public DownloadApplicationActor(
            IDownloadApplicationSettings downloadApplicationSettings,
            IHttpClientFactory httpClientFactory,
            IHostingEnvironment hostingEnvironment,
            string getBuildsUrl,
            string getBuildDownloadLinkTemplateUrl)
        {
            this.getBuildsUrl = getBuildsUrl;
            this.getBuildDownloadLinkTemplateUrl = getBuildDownloadLinkTemplateUrl;
            this.hostingEnvironment = hostingEnvironment;
            this.downloadApplicationSettings = downloadApplicationSettings;
            this.httpClientFactory = httpClientFactory;
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
            this.Sender.Tell(new GetLatestApplicationBuildPath.Response(this.latestBuildPath));
        }

        private async Task DownloadBuild()
        {
            var latestBuild = await this.GetLatestBuild();
            if (latestBuild == null)
            {
                this.logger.Warning($"No builds found for url {this.getBuildsUrl}");
                return;
            }

            if (this.latestBuildNumber == latestBuild.Id)
            {
                return;
            }

            var buildDownloadModel = await this.GetBuildDownloadModel(latestBuild);

            using (var client = this.httpClientFactory.CreateClient())
            {
                var buildFileStream = await client.GetStreamAsync(buildDownloadModel.Uri);
                var newBuildPath = await this.SaveBuildFile(buildFileStream, buildDownloadModel);

                this.latestBuildNumber = latestBuild.Id;
                this.latestBuildPath = newBuildPath;
            }
        }

        private async Task<AppCenterBuildModel> GetLatestBuild()
        {
            using (var response = await this.SendAppCenterRequest(this.getBuildsUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var builds = this.DeserializeJson<IEnumerable<AppCenterBuildModel>>(contentString);

                return builds
                    .Where(b => b.Result == "succeeded" && b.Status == "completed")
                    .OrderByDescending(b => b.FinishTime)
                    .FirstOrDefault();
            }
        }

        private async Task<AppCenterBuildDownloadModel> GetBuildDownloadModel(AppCenterBuildModel build)
        {
            var getBuildUrl = getBuildDownloadLinkTemplateUrl.Replace("{buildId}", build.Id.ToString());

            using (var response = await this.SendAppCenterRequest(getBuildUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();

                var downloadModel = this.DeserializeJson<AppCenterBuildDownloadModel>(contentString);
                downloadModel.BuildNumber = build.Id;

                return downloadModel;
            }
        }

        private async Task<string> SaveBuildFile(Stream fileStream, AppCenterBuildDownloadModel buildDownloadModel)
        {
            var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            var fileEntry = zipArchive.Entries.Single(e => !string.IsNullOrEmpty(e.Name));

            var fileName = this.downloadApplicationSettings.RenameBuildFilePattern
                .Replace("{buildNumber}", buildDownloadModel.BuildNumber.ToString())
                .Replace("{ext}", Path.GetExtension(fileEntry.Name));

            var buildsPath = Path.Combine(
                this.hostingEnvironment.ContentRootPath,
                this.downloadApplicationSettings.BuildsFolder);
            if (!Directory.Exists(buildsPath))
            {
                Directory.CreateDirectory(buildsPath);
            }

            var filePath = Path.Combine(
                buildsPath,
                fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            using (var stream = fileEntry.Open())
            {
                using (var file = File.Create(filePath))
                {
                    await stream.CopyToAsync(file);
                }
            }

            return filePath;
        }

        private async Task<HttpResponseMessage> SendAppCenterRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-API-Token", this.downloadApplicationSettings.ApiToken);

            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                return await httpClient.SendAsync(request);
            }
        }

        private T DeserializeJson<T>(string message)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(message, serializerSettings);
        }
    }
}
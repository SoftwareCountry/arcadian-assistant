namespace Arcadia.Assistant.Web.Download
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadActor : UntypedActor, ILogReceive
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;
        private readonly IHttpClientFactory httpClientFactory;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public DownloadActor(IDownloadApplicationSettings downloadApplicationSettings, IHttpClientFactory httpClientFactory)
        {
            this.downloadApplicationSettings = downloadApplicationSettings;
            this.httpClientFactory = httpClientFactory;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(downloadApplicationSettings.DownloadBuildIntervalMinutes),
                this.Self,
                RefreshApplicationBuilds.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshApplicationBuilds _:
                    this.DownloadApplicationBuilds()
                        .PipeTo(
                            this.Self,
                            success: () => RefreshApplicationBuildsFinish.Instance,
                            failure: err => new RefreshApplicationBuildsFinish(err.Message));
                    break;

                case RefreshApplicationBuildsFinish msg:
                    if (msg.Message != null)
                    {
                        this.logger.Warning(msg.Message);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task DownloadApplicationBuilds()
        {
            var androidDownloadBuildTask = this.DownloadBuild(
                this.downloadApplicationSettings.AndroidGetBuildsUrl,
                this.downloadApplicationSettings.AndroidGetBuildDownloadLinkTemplateUrl);

            var iosDownloadBuildTask = this.DownloadBuild(
                this.downloadApplicationSettings.IosGetBuildsUrl,
                this.downloadApplicationSettings.IosGetBuildDownloadLinkTemplateUrl);

            await Task.WhenAll(androidDownloadBuildTask, iosDownloadBuildTask);
        }

        private async Task DownloadBuild(string getBuildsUrl, string getBuildDownloadLinkTemplateUrl)
        {
            var latestBuild = await this.GetLatestBuild(getBuildsUrl);
            if (latestBuild == null)
            {
                this.logger.Warning($"No builds found for url {getBuildsUrl}");
                return;
            }

            var buildDownloadModel = await this.GetBuildDownloadModel(getBuildDownloadLinkTemplateUrl, latestBuild);

            using (var client = this.httpClientFactory.CreateClient())
            {
                var buildFileStream = await client.GetStreamAsync(buildDownloadModel.Uri);
                this.SaveBuildFile(buildFileStream, buildDownloadModel);
            }
        }

        private async Task<AppCenterBuildModel> GetLatestBuild(string getBuildsUrl)
        {
            using (var response = await this.SendAppCenterRequest(getBuildsUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var builds = this.DeserializeJson<IEnumerable<AppCenterBuildModel>>(contentString);

                return builds
                    .OrderByDescending(b => b.FinishTime)
                    .FirstOrDefault();
            }
        }

        private async Task<AppCenterBuildDownloadModel> GetBuildDownloadModel(string getBuildDownloadLinkTemplateUrl, AppCenterBuildModel build)
        {
            var getBuildUrl = getBuildDownloadLinkTemplateUrl.Replace("{buildId}", build.Id.ToString());
            using (var response = await this.SendAppCenterRequest(getBuildUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();

                var downloadModel = this.DeserializeJson<AppCenterBuildDownloadModel>(contentString);
                downloadModel.BuildDate = build.FinishTime;

                return downloadModel;
            }
        }

        private void SaveBuildFile(Stream fileStream, AppCenterBuildDownloadModel buildDownloadModel)
        {
            var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            var fileEntry = zipArchive.Entries.Single(e => !string.IsNullOrEmpty(e.Name));

            using (var stream = fileEntry.Open())
            {
                var fileName = this.downloadApplicationSettings.RenameBuildFilePattern
                    .Replace("{date}", buildDownloadModel.BuildDate.ToString("yyyy-MM-dd-hh-mm-ss"))
                    .Replace("{ext}", Path.GetExtension(fileEntry.Name));

                var filePath = Path.Combine(this.downloadApplicationSettings.BuildsFolder, fileName);
                if (File.Exists(filePath))
                {
                    return;
                }

                using (var file = File.Create(filePath))
                {
                    stream.CopyTo(file);
                }
            }
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

        private class RefreshApplicationBuilds
        {
            public static readonly RefreshApplicationBuilds Instance = new RefreshApplicationBuilds();
        }

        private class RefreshApplicationBuildsFinish
        {
            public static readonly RefreshApplicationBuildsFinish Instance = new RefreshApplicationBuildsFinish();

            public RefreshApplicationBuildsFinish(string message = null)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}
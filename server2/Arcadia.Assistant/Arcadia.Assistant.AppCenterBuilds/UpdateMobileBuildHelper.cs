using Arcadia.Assistant.AppCenterBuilds.Contracts.AppCenter;
using Arcadia.Assistant.MobileBuild.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Assistant.AppCenterBuilds
{
    internal class UpdateMobileBuildHelper
    {
        private readonly string buildUrl;
        private readonly string downloadUrlTemplate;
        private readonly string apiKey;

        public UpdateMobileBuildHelper(string? buildUrl, string? downloadUrlTemplate, string? apiKey)
        {
            if (buildUrl == null || downloadUrlTemplate == null)
                throw new ArgumentNullException("Mobile build configuration error.");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("Mobile build ApiKey is empty.");

            this.buildUrl = buildUrl;
            this.downloadUrlTemplate = downloadUrlTemplate;
            this.apiKey = apiKey;
        }

        #region public mehods

        public async Task<bool> CheckAndupdateMobileBuild(IHttpClientFactory httpClientFactory, IMobileBuildActor mobileBuildActor, CancellationToken cancellationToken, Action<string> logStore)
        {
            var currentMobileBuildVersion = await mobileBuildActor.GetMobileBuildVersionAsync(cancellationToken);
            var appCenterLatestBuild = await GetLatestBuild(httpClientFactory);
            if (!appCenterLatestBuild.Id.HasValue)
            {
                throw new ArgumentNullException("Application center build identifier expected");
            }
            var appCenterLastBuildVersion = appCenterLatestBuild.Id.Value.ToString();

            if (currentMobileBuildVersion != appCenterLastBuildVersion)
            {
                var downloadModel = await GetBuildDownloadModel(appCenterLatestBuild, httpClientFactory);
                var data = await GetBuildData(downloadModel, httpClientFactory);
                await mobileBuildActor.SetMobileBuildData(appCenterLastBuildVersion, data, cancellationToken);
                logStore($"Mobile build {appCenterLastBuildVersion} updated from {downloadModel.Uri}");
            }
            else
            {
                // TO DO: Refactor for common logger using
                logStore("The same version - nothing to do");
            }

            return true;
        }

        #endregion

        #region private methods

        private async Task<AppCenterBuildModel> GetLatestBuild(IHttpClientFactory httpClientFactory)
        {
            using (var response = await this.SendAppCenterRequest(this.buildUrl, httpClientFactory))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var builds = this.DeserializeJson<IEnumerable<AppCenterBuildModel>>(contentString);

                return builds
                    .Where(b => b.Result == "succeeded" && b.Status == "completed")
                    .OrderByDescending(b => b.FinishTime)
                    .FirstOrDefault();
            }
        }

        private async Task<AppCenterBuildDownloadModel> GetBuildDownloadModel(AppCenterBuildModel build, IHttpClientFactory httpClientFactory)
        {
            var getBuildUrl = this.downloadUrlTemplate.Replace("{buildId}", build.Id.ToString());

            using (var response = await this.SendAppCenterRequest(getBuildUrl, httpClientFactory))
            {
                var contentString = await response.Content.ReadAsStringAsync();

                var downloadModel = this.DeserializeJson<AppCenterBuildDownloadModel>(contentString);
                downloadModel.BuildNumber = build.Id;

                return downloadModel;
            }
        }

        private async Task<byte[]> GetBuildData(AppCenterBuildDownloadModel buildDownloadModel, IHttpClientFactory httpClientFactory)
        {
            using (var client = httpClientFactory.CreateClient())
            {
                return await client.GetByteArrayAsync(buildDownloadModel.Uri);
            }
        }

        private async Task<HttpResponseMessage> SendAppCenterRequest(string url, IHttpClientFactory httpClientFactory)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-API-Token", this.apiKey);

            using (var httpClient = httpClientFactory.CreateClient())
            {
                return await httpClient.SendAsync(request);
            }
        }

        private T DeserializeJson<T>(string message)
        {
            var serializerSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<T>(message, serializerSettings);
        }

        #endregion
    }
}

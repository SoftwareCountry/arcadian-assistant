using Arcadia.Assistant.AppCenterBuilds.Contracts.AppCenter;
using Arcadia.Assistant.MobileBuild.Contracts.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Assistant.AppCenterBuilds
{
    internal class UpdateMobileBuildHelper
    {
        private readonly string buildUrl;
        private readonly string downloadUrlTemplate;
        private readonly string apiKey;

        public UpdateMobileBuildHelper(string buildUrl, string downloadUrlTemplate, string apiKey)
        {
            this.buildUrl = buildUrl;
            this.downloadUrlTemplate = downloadUrlTemplate;
            this.apiKey = apiKey;
        }

        #region public mehods

        public async Task<bool> CheckAndupdateMobileBuild(IHttpClientFactory httpClientFactory, IMobileBuildActor mobileBuildActor, CancellationToken cancellationToken, Action<string> logStore)
        {
            var currentMobileBuildVersion = await mobileBuildActor.GetMobileBuildVersionAsync(cancellationToken);
            var appCenterLatestBuild = await GetLatestBuild(httpClientFactory);
            var appCenterLastBuildVersion = appCenterLatestBuild.Id.ToString();

            if (currentMobileBuildVersion != appCenterLastBuildVersion)
            {
                var downloadModel = await GetBuildDownloadModel(appCenterLatestBuild, httpClientFactory);
                var data = await GetBuildData(downloadModel, httpClientFactory);
                await mobileBuildActor.SetMobileBuildData(appCenterLastBuildVersion, data, cancellationToken);
                logStore?.Invoke($"Mobile build {appCenterLastBuildVersion} updated from {downloadModel.Uri}");
            }
            else
            {
                logStore?.Invoke("The same version - nothing to do");
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
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(message, serializerSettings);
        }

        #endregion
    }
}

namespace Arcadia.Assistant.AppCenterBuilds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using MobileBuild.Contracts;

    using Models;

    public class UpdateMobileBuildHelper
    {
        private readonly string apiKey;
        private readonly string buildUrl;
        private readonly string downloadUrlTemplate;
        private readonly ILogger logger;

        public UpdateMobileBuildHelper(string buildUrl, string downloadUrlTemplate, string apiKey, ILogger logger)
        {
            this.buildUrl = buildUrl;
            this.downloadUrlTemplate = downloadUrlTemplate;
            this.apiKey = apiKey;
            this.logger = logger;
        }

        #region public mehods

        public async Task CheckAndUpdateMobileBuild(
            IHttpClientFactory httpClientFactory, IMobileBuildActor mobileBuildActor, string deviceType,
            IAppCenterNotification notification, CancellationToken cancellationToken)
        {
            var currentMobileBuildVersion = await mobileBuildActor.GetMobileBuildVersionAsync(cancellationToken);
            var appCenterLatestBuild = await this.GetLatestBuild(httpClientFactory);
            if (!appCenterLatestBuild.Id.HasValue)
            {
                throw new Exception("Application center build identifier expected");
            }

            var appCenterLastBuildVersion = appCenterLatestBuild.Id.Value.ToString();

            if (currentMobileBuildVersion != appCenterLastBuildVersion)
            {
                var downloadModel = await this.GetBuildDownloadModel(appCenterLatestBuild, httpClientFactory);
                var data = await this.GetBuildData(downloadModel, httpClientFactory);
                await mobileBuildActor.SetMobileBuildData(appCenterLastBuildVersion, data, cancellationToken);
                this.logger.LogInformation("Mobile build {BuildVersion} updated from {Uri}", appCenterLastBuildVersion,
                    downloadModel.Uri);
                await notification.Notify("NewMobileVersion", appCenterLastBuildVersion, deviceType, cancellationToken);
            }
            else
            {
                this.logger.LogInformation("The same version - nothing to do");
            }
        }

        #endregion

        #region private methods

        private async Task<AppCenterBuildModel> GetLatestBuild(IHttpClientFactory httpClientFactory)
        {
            using var response = await this.SendAppCenterRequest(this.buildUrl, httpClientFactory);

            var contentString = await response.Content.ReadAsStringAsync();
            var builds = this.DeserializeJson<IEnumerable<AppCenterBuildModel>>(contentString);

            return builds
                .Where(b => b.Result == "succeeded" && b.Status == "completed")
                .OrderByDescending(b => b.FinishTime)
                .FirstOrDefault();
        }

        private async Task<AppCenterBuildDownloadModel> GetBuildDownloadModel(
            AppCenterBuildModel build, IHttpClientFactory httpClientFactory)
        {
            var getBuildUrl = this.downloadUrlTemplate.Replace("{buildId}", build.Id.ToString());

            using var response = await this.SendAppCenterRequest(getBuildUrl, httpClientFactory);

            var contentString = await response.Content.ReadAsStringAsync();

            var downloadModel = this.DeserializeJson<AppCenterBuildDownloadModel>(contentString);
            downloadModel.BuildNumber = build.Id;

            return downloadModel;
        }

        private async Task<byte[]> GetBuildData(
            AppCenterBuildDownloadModel buildDownloadModel, IHttpClientFactory httpClientFactory)
        {
            using var client = httpClientFactory.CreateClient();
            return await client.GetByteArrayAsync(buildDownloadModel.Uri);
        }

        private async Task<HttpResponseMessage> SendAppCenterRequest(string url, IHttpClientFactory httpClientFactory)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-API-Token", this.apiKey);

            using var httpClient = httpClientFactory.CreateClient();
            return await httpClient.SendAsync(request);
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
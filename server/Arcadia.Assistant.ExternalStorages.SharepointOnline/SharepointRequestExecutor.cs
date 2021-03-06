﻿namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    using Newtonsoft.Json;

    public class SharepointRequestExecutor : ISharepointRequestExecutor
    {
        private readonly ISharepointOnlineConfiguration configuration;
        private readonly ISharepointAuthTokenService authTokenService;
        private readonly HttpClient httpClient;

        private string accessToken;

        public SharepointRequestExecutor(
            ISharepointOnlineConfiguration configuration,
            ISharepointAuthTokenService authTokenService,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.authTokenService = authTokenService;
            this.httpClient = httpClientFactory.CreateClient();
        }

        public async Task<T> ExecuteSharepointRequest<T>(SharepointRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await this.ExecuteSharepointRequest(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with {response.StatusCode} status code");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<HttpResponseMessage> ExecuteSharepointRequest(SharepointRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            // To cache access token for several Sharepoint requests in bounds of one request to storage
            if (this.accessToken == null)
            {
                this.accessToken = await this.authTokenService.GetAccessToken(this.configuration.ServerUrl, cancellationToken);
            }

            request = request
                .WithAcceptHeader("application/json;odata=nometadata")
                .WithBearerAuthorizationHeader(this.accessToken);

            return await this.httpClient.SendAsync(request.GetHttpRequest(), cancellationToken);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
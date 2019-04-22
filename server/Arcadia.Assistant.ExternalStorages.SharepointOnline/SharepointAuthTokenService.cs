namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    using Newtonsoft.Json;

    public class SharepointAuthTokenService : ISharepointAuthTokenService
    {
        private const string SharepointPrincipal = "00000003-0000-0ff1-ce00-000000000000";

        private const string AcsMetadataEndpoint = "https://accounts.accesscontrol.windows.net/metadata/json/1";
        private const string OAuth2GrantType = "client_credentials";
        private const string OAuth2Protocol = "OAuth2";

        private readonly ISharepointOnlineConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public SharepointAuthTokenService(ISharepointOnlineConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAccessToken(string sharepointUrl, CancellationToken cancellationToken)
        {
            var realm = await this.GetRealm(sharepointUrl, cancellationToken);
            var hostName = new Uri(sharepointUrl).Authority;

            var oauthClientId = this.GetFormattedPrincipal(this.configuration.ClientId, null, realm);
            var oauthResource = this.GetFormattedPrincipal(SharepointPrincipal, hostName, realm);

            var accessToken = await this.IssueToken(
                realm,
                oauthClientId,
                this.configuration.ClientSecret,
                oauthResource,
                cancellationToken);
            return accessToken;
        }

        private async Task<string> GetRealm(string sharepointUrl, CancellationToken cancellationToken)
        {
            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, sharepointUrl + "/_vti_bin/client.svc");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", string.Empty);

                var response = await httpClient.SendAsync(request, cancellationToken);

                if (response.StatusCode != HttpStatusCode.Unauthorized || response.Headers.WwwAuthenticate == null)
                {
                    throw new HttpRequestException("Not supported response to a realm request");
                }

                var wwwAuthenticateHeaderValue = response.Headers.WwwAuthenticate.ToString();

                var realm = this.GetRealmFromHeader(wwwAuthenticateHeaderValue);

                if (realm == null)
                {
                    throw new HttpRequestException("Not supported response to a realm request");
                }

                return realm;
            }
        }

        private string GetRealmFromHeader(string headerValue)
        {
            const string bearer = "Bearer realm=\"";
            const int realmLength = 36;

            var bearerIndex = headerValue.IndexOf(bearer, StringComparison.Ordinal);

            if (bearerIndex < 0)
            {
                return null;
            }

            var realmStartIndex = bearerIndex + bearer.Length;

            if (headerValue.Length < realmStartIndex + realmLength)
            {
                return null;
            }

            var realm = headerValue.Substring(realmStartIndex, realmLength);

            return !Guid.TryParse(realm, out _) ? null : realm;
        }

        private async Task<string> IssueToken(
            string realm,
            string tokenClientId,
            string tokenClientSecret,
            string tokenResource,
            CancellationToken cancellationToken)
        {
            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                var tokenUrl = await this.GetOAuth2Url(realm, cancellationToken);

                var requestData =
                    "grant_type=" + HttpUtility.UrlEncode(OAuth2GrantType) +
                    "&client_id=" + HttpUtility.UrlEncode(tokenClientId) +
                    "&client_secret=" + HttpUtility.UrlEncode(tokenClientSecret) +
                    "&resource=" + HttpUtility.UrlEncode(tokenResource);

                var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
                {
                    Content = new StringContent(requestData)
                };

                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Content.Headers.ContentLength = requestData.Length;

                var response = await httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Access token response has failed");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var oauthResult = JsonConvert.DeserializeObject<OAuth2AccessTokenResponse>(responseContent);
                return oauthResult.AccessToken;
            }
        }

        private async Task<string> GetOAuth2Url(string realm, CancellationToken cancellationToken)
        {
            var metadata = await this.GetMetadataDocument(realm, cancellationToken);

            var s2sEndpoint = metadata.Endpoints.SingleOrDefault(e => e.Protocol == OAuth2Protocol);

            if (s2sEndpoint != null)
            {
                return s2sEndpoint.Location;
            }

            throw new Exception("Metadata document does not contain OAuth2 endpoint url");
        }

        private async Task<JsonMetadataDocument> GetMetadataDocument(string realm, CancellationToken cancellationToken)
        {
            using (var httpClient = this.httpClientFactory.CreateClient())
            {
                var metadataEndpointUrl = $"{AcsMetadataEndpoint}?realm={realm}";

                var response = await httpClient.GetAsync(metadataEndpointUrl, cancellationToken);
                var metadataString = await response.Content.ReadAsStringAsync();
                var metadata = JsonConvert.DeserializeObject<JsonMetadataDocument>(metadataString);

                if (metadata == null)
                {
                    throw new Exception($"No metadata document found at the global endpoint {metadataEndpointUrl}");
                }

                return metadata;
            }
        }

        private string GetFormattedPrincipal(string principalName, string hostName, string realm)
        {
            return string.IsNullOrEmpty(hostName)
                ? string.Format(CultureInfo.InvariantCulture, "{0}@{1}", principalName, realm)
                : string.Format(CultureInfo.InvariantCulture, "{0}/{1}@{2}", principalName, hostName, realm);
        }

        [DataContract]
        private class JsonMetadataDocument
        {
            [DataMember(Name = "endpoints")]
            public IEnumerable<JsonEndpoint> Endpoints { get; set; }
        }

        [DataContract]
        private class JsonEndpoint
        {
            [DataMember(Name = "location")]
            public string Location { get; set; }

            [DataMember(Name = "protocol")]
            public string Protocol { get; set; }
        }

        [DataContract]
        private class OAuth2AccessTokenResponse
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
        }
    }
}
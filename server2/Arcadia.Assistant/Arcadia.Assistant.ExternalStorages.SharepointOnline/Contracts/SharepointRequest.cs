namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;

    public class SharepointRequest
    {
        private const string AcceptHeaderName = "Accept";
        private const string AuthorizationHeaderName = "Authorization";
        private const string ContentTypeHeaderName = "Content-Type";
        private const string ContentLengthHeaderName = "Content-Length";
        private const string IfMatchHeaderName = "IF-MATCH";
        private const string XHttpMethodHeaderName = "X-HTTP-Method";

        /**** According rfc7230 3.2.2 p.2 
         * A sender must not generate multiple header fields with the same field name in a message unless either the entire field
         * value for that header field is defined as a comma-separated list [i.e., #(values)] or the header field is a well-known
         * exception (as noted below).
         */
        private readonly Dictionary<string, List<string>> headersInternal = new Dictionary<string, List<string>>();

        private SharepointRequest(HttpMethod httpMethod, string url)
        {
            this.HttpMethod = httpMethod;
            this.Url = url;
        }

        public HttpMethod HttpMethod { get; }

        public string Url { get; }

        public HttpContent? Content { get; private set; }

        public static SharepointRequest Create(HttpMethod httpMethod, string url)
        {
            return new SharepointRequest(httpMethod, url);
        }

        public SharepointRequest WithAcceptHeader(string value)
        {
            this.AddHeader(AcceptHeaderName, value);
            return this;
        }

        public SharepointRequest WithBearerAuthorizationHeader(string value)
        {
            this.AddHeader(AuthorizationHeaderName, value);
            return this;
        }

        public SharepointRequest WithContentString(string contentString)
        {
            this.Content = new StringContent(contentString);

            this.AddHeader(ContentTypeHeaderName, "application/json;odata=verbose");
            this.AddHeader(ContentLengthHeaderName, contentString.Length.ToString());

            return this;
        }

        public SharepointRequest WithContent(object content)
        {
            var contentString = JsonSerializer.Serialize(content);
            return this.WithContentString(contentString);
        }

        public SharepointRequest WithIfMatchHeader()
        {
            this.AddHeader(IfMatchHeaderName, "*");
            return this;
        }

        public SharepointRequest WithXHttpMethodHeader(string value)
        {
            this.AddHeader(XHttpMethodHeaderName, value);
            return this;
        }

        public HttpRequestMessage GetHttpRequest()
        {
            var httpRequest = new HttpRequestMessage(this.HttpMethod, this.Url);

            if (this.Content != null)
            {
                httpRequest.Content = this.Content;
            }

            foreach (var (headerName, headerValues) in this.headersInternal)
            {
                var headerValue = string.Join(", ", headerValues);
                switch (headerName)
                {
                    case AcceptHeaderName:
                        httpRequest.Headers.Accept.Clear();
                        httpRequest.Headers.Accept.TryParseAdd(headerValue);
                        break;

                    case AuthorizationHeaderName:
                        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headerValue);
                        break;

                    case ContentTypeHeaderName:
                        httpRequest.Content.Headers.Remove(headerName);
                        httpRequest.Content.Headers.TryAddWithoutValidation(headerName, headerValue);
                        break;

                    case ContentLengthHeaderName:
                        httpRequest.Content.Headers.ContentLength = long.Parse(headerValue);
                        break;

                    default:
                        httpRequest.Headers.TryAddWithoutValidation(headerName, headerValue);
                        break;
                }
            }

            return httpRequest;
        }

        private void AddHeader(string name, string value)
        {
            if (!this.headersInternal.TryGetValue(name, out var headerValues))
            {
                this.headersInternal.Add(name, new List<string>()
                {
                    value
                });
            } else if (!headerValues.Contains(value))
            {
                this.headersInternal[name].Add(value);
            }
        }
    }
}
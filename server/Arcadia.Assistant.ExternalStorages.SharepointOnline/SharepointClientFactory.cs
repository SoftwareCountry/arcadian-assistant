namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;

    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    using Microsoft.SharePoint.Client;

    public class SharepointClientFactory : ISharepointClientFactory
    {
        public ClientContext GetClient(string url)
        {
            var uri = new Uri(url);

            var realm = TokenHelper.GetRealmFromTargetUrl(uri);
            var accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, uri.Authority, realm).AccessToken;

            return TokenHelper.GetClientContextWithAccessToken(url, accessToken);
        }
    }
}
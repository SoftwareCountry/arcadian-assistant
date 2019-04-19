namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using Microsoft.SharePoint.Client;

    public interface ISharepointClientFactory
    {
        ClientContext GetClient(string url);
    }
}
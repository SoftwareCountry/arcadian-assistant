namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    public interface ISharepointOnlineConfiguration
    {
        string ServerUrl { get; }

        string ClientId { get; }

        string ClientSecret { get; }
    }
}
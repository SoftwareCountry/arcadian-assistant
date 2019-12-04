namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using Contracts;

    public class SharepointOnlineConfiguration : ISharepointOnlineConfiguration
    {
        public string ServerUrl { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;
    }
}
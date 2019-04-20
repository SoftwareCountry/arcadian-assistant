namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;

    public class SharepointOnlineConfiguration : ISharepointOnlineConfiguration
    {
        public string ServerUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
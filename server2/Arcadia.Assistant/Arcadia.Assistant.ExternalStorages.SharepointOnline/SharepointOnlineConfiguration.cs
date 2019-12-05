namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System.Fabric.Description;

    using Contracts;

    public class SharepointOnlineConfiguration : ISharepointOnlineConfiguration
    {
        public SharepointOnlineConfiguration(ConfigurationSection configurationSection)
        {
            this.ServerUrl = configurationSection.Parameters["ServerUrl"].Value;
            this.ClientId = configurationSection.Parameters["ClientId"].Value;
            this.ClientSecret = configurationSection.Parameters["ClientSecret"].Value;
        }

        public string ServerUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
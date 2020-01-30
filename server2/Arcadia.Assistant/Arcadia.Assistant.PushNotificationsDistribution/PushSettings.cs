namespace Arcadia.Assistant.PushNotificationsDistributor
{
    using System.Fabric.Description;

    public class PushSettings : IPushSettings
    {
        public PushSettings(ConfigurationSection configurationSection)
        {
            if (bool.TryParse(configurationSection.Parameters["Enabled"].Value, out var enable))
            {
                this.Enabled = enable;
            }

            this.ApiToken = configurationSection.Parameters["ApiToken"].Value;
            this.AndroidPushUrl = configurationSection.Parameters["AndroidPushUrl"].Value;
            this.IosPushUrl = configurationSection.Parameters["IosPushUrl"].Value;
        }

        public bool Enabled { get; set; }

        public string ApiToken { get; set; }

        public string AndroidPushUrl { get; set; }

        public string IosPushUrl { get; set; }
    }
}
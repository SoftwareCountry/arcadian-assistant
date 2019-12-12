namespace Arcadia.Assistant.Sharepoint
{
    using System.Fabric.Description;

    public class SharepointSynchronizationSettings : ISharepointSynchronizationSettings
    {
        private readonly int DefaultSynchronizationIntervalMinutes = 240;

        public SharepointSynchronizationSettings(ConfigurationSection configurationSection)
        {
            this.SynchronizationIntervalMinutes = int.TryParse(configurationSection.Parameters["SynchronizationIntervalMinutes"].Value, out var interval) ? interval : this.DefaultSynchronizationIntervalMinutes;
        }

        public int SynchronizationIntervalMinutes { get; }
    }
}
namespace Arcadia.Assistant.EmailNotifications.Models
{
    using System.Fabric.Description;

    public class EmailNotificationSettings
    {
        public EmailNotificationSettings(ConfigurationSection configurationSection)
        {
            if (configurationSection.Parameters["ArcadiaAssistantFrom"].Value != null)
            {
                this.ArcadiaAssistantFrom = configurationSection.Parameters["ArcadiaAssistantFrom"].Value;
            }
        }

        public string ArcadiaAssistantFrom { get; } = string.Empty;
    }
}
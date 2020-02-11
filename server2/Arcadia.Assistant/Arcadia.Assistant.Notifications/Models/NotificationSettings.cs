namespace Arcadia.Assistant.Notifications.Models
{
    using System.Fabric.Description;

    using Interfaces;

    public class NotificationSettings : INotificationSettings
    {
        public NotificationSettings(ConfigurationSection configurationSection)
        {
            if (bool.TryParse(configurationSection.Parameters["EnablePush"].Value, out var enable))
            {
                this.EnablePush = enable;
            }
        }

        public bool EnablePush { get; set; }
    }
}
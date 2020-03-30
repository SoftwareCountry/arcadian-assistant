namespace Arcadia.Assistant.EmailNotifications.Models
{
    using System.Fabric.Description;

    public class SmtpSettings
    {
        public SmtpSettings(ConfigurationSection configurationSection)
        {
            if (int.TryParse(configurationSection.Parameters["Port"].Value, out var port))
            {
                this.Port = port;
            }

            if (configurationSection.Parameters["Host"].Value != null)
            {
                this.Host = configurationSection.Parameters["Host"].Value;
            }

            if (configurationSection.Parameters["UserName"].Value != null)
            {
                this.UserName = configurationSection.Parameters["UserName"].Value;
            }

            if (configurationSection.Parameters["Password"].Value != null)
            {
                this.Password = configurationSection.Parameters["Password"].Value;
            }

            if (bool.TryParse(configurationSection.Parameters["UseTls"].Value, out var enable))
            {
                this.UseTls = enable;
            }
        }

        public int Port { get; }

        public string Host { get; } = string.Empty;

        public string UserName { get; } = string.Empty;

        public string Password { get; } = string.Empty;

        public bool UseTls { get; }
    }
}
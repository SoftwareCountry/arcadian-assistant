namespace Arcadia.Assistant.Inbox
{
    using System.Fabric.Description;

    public class ImapConfiguration
    {
        public ImapConfiguration(ConfigurationSection configurationSection)
        {
            this.Host = configurationSection.Parameters["Host"].Value;
            this.Port = int.Parse(configurationSection.Parameters["Port"].Value);
            this.User = configurationSection.Parameters["User"].Value;
            this.Password = configurationSection.Parameters["Password"].Value;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }
}
namespace Arcadia.Assistant.VacationsCredit
{
    using System;
    using System.Fabric.Description;

    public class InboxConfiguration
    {
        public InboxConfiguration(ConfigurationSection configurationSection)
        {
            this.Enabled = bool.Parse(configurationSection.Parameters["Enabled"].Value);
            this.Subject = configurationSection.Parameters["Subject"].Value;
            this.Sender = configurationSection.Parameters["Sender"].Value;
            this.RefreshInterval = TimeSpan.FromMinutes(int.Parse(configurationSection.Parameters["RefreshIntervalMinutes"].Value));
        }

        public bool Enabled { get; set; }

        public string Sender { get; set; }

        public string Subject { get; set; }

        public TimeSpan RefreshInterval { get; set; }
    }
}
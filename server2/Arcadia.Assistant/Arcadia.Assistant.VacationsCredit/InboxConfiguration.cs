namespace Arcadia.Assistant.VacationsCredit
{
    using System.Fabric.Description;

    public class InboxConfiguration
    {
        public InboxConfiguration()
        {
        }

        public InboxConfiguration(ConfigurationSection configurationSection)
        {
            this.Enabled = bool.Parse(configurationSection.Parameters["Enabled"].Value);
            this.Subject = configurationSection.Parameters["Subject"].Value;
            this.Sender = configurationSection.Parameters["Sender"].Value;
        }

        public bool Enabled { get; set; }

        public string Sender { get; set; }

        public string Subject { get; set; }
    }
}
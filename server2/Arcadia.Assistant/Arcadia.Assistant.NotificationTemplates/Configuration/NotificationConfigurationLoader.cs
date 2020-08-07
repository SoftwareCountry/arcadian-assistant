namespace Arcadia.Assistant.NotificationTemplates.Configuration
{
    using System.Fabric.Description;

    public static class NotificationConfigurationLoader
    {
        public static T Load<T>(ConfigurationSection section) where T : INotificationConfiguration
        {
            INotificationConfiguration result = new NotificationConfiguration(section);
            return (T)result;
        }

        internal class NotificationConfiguration : INotificationConfiguration
        {
            public NotificationConfiguration(ConfigurationSection section)
            {
                this.Subject = section.Parameters["Subject"].Value;
                this.Title = section.Parameters["Title"].Value;
                this.ShortBodyTemplate = section.Parameters["ShortBodyTemplate"].Value;
                this.LongBodyTemplate = section.Parameters["LongBodyTemplate"].Value;
            }

            public string Subject { get; }

            public string Title { get; }

            public string ShortBodyTemplate { get; }

            public string LongBodyTemplate { get; }
        }
    }
}
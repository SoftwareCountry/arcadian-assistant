namespace Arcadia.Assistant.Notifications.Models
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Description;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Extensions.Logging;

    public class NotificationSettings
    {
        private readonly ILogger logger;

        public NotificationSettings(ConfigurationSection configurationSection, ILogger<NotificationSettings> logger)
        {
            this.logger = logger;

            if (bool.TryParse(configurationSection.Parameters["EnablePush"].Value, out var enablePush))
            {
                this.EnablePush = enablePush;
            }

            if (bool.TryParse(configurationSection.Parameters["EnableEmail"].Value, out var enableEmail))
            {
                this.EnableEmail = enableEmail;
            }

            var map = configurationSection.Parameters["ClientNotificationProviders"].Value;
            if (!string.IsNullOrEmpty(map))
            {
                var parsedMap = this.LoadNotificationProvidersMap(map);
                this.NotificationTemplateProvidersMap = parsedMap
                    .GroupBy(x => x.Item1)
                    .ToDictionary(x => x.Key,
                        x => (IReadOnlyCollection<NotificationType>)x
                            .SelectMany(v => v.Item2)
                            .Distinct()
                            .ToList()
                            .AsReadOnly());
            }
        }

        public bool EnablePush { get; }

        public bool EnableEmail { get; }

        public IReadOnlyDictionary<string, IReadOnlyCollection<NotificationType>> NotificationTemplateProvidersMap
        {
            get;
        }
            = new Dictionary<string, IReadOnlyCollection<NotificationType>>();

        private IEnumerable<(string, IEnumerable<NotificationType>)> LoadNotificationProvidersMap(string map)
        {
            var result = new List<(string, IEnumerable<NotificationType>)>();
            try
            {
                using var reader =
                    JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(map),
                        new XmlDictionaryReaderQuotas());
                var mapConfig = XElement.Load(reader);
                foreach (var item in mapConfig.Elements())
                {
                    var clientName = ((XElement)item.FirstNode).Name.LocalName;
                    var values = ((XElement)item.FirstNode).Elements()
                        .Select(x => x.Value)
                        .Distinct()
                        .Where(n => Enum.TryParse<NotificationType>(n, true, out var val))
                        .Select(n => Enum.Parse<NotificationType>(n, true));
                    result.Add((clientName, values.ToList()));
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Notification providers map parse error");
                result = new List<(string, IEnumerable<NotificationType>)>();
            }

            return result;
        }
    }
}
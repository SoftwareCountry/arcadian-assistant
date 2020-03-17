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

    using Contracts.Models;

    using Interfaces;

    public class NotificationSettings : INotificationSettings
    {
        public NotificationSettings(ConfigurationSection configurationSection)
        {
            if (bool.TryParse(configurationSection.Parameters["EnablePush"].Value, out var enable))
            {
                this.EnablePush = enable;
            }

            var map = configurationSection.Parameters["ClientNotificationProviders"].Value;
            if (!string.IsNullOrEmpty(map))
            {
                var parsedMap = this.LoadNotificationProvidersMap(map);
                this.ClientNotificationProvidersMap = parsedMap
                    .GroupBy(x => x.Item1)
                    .ToDictionary(x => x.Key,
                        x => x.SelectMany(v => v.Item2).Distinct());
            }
        }

        public bool EnablePush { get; }

        public Dictionary<string, IEnumerable<NotificationType>> ClientNotificationProvidersMap { get; } = new Dictionary<string, IEnumerable<NotificationType>>();

        private IEnumerable<Tuple<string, IEnumerable<NotificationType>>> LoadNotificationProvidersMap(string map)
        {
            var result = new List<Tuple<string, IEnumerable<NotificationType>>>();
            try
            {
                using var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(map), new XmlDictionaryReaderQuotas());
                var mapConfig = XElement.Load(reader);
                foreach (var item in mapConfig.Elements())
                {
                    var clientName = ((XElement)item.FirstNode).Name.LocalName;
                    var values = ((XElement)item.FirstNode).Elements()
                        .Select(x => x.Value)
                        .Distinct()
                        .Where(n => Enum.TryParse<NotificationType>(n, true, out var val))
                        .Select(n => Enum.Parse<NotificationType>(n, true));
                    result.Add(new Tuple<string, IEnumerable<NotificationType>>(clientName, values.ToList()));
                }
            }
            catch (Exception e)
            {
                result = new List<Tuple<string, IEnumerable<NotificationType>>>();
            }

            return result;
        }

        private class NotificationProviderMapSetting : Tuple<string, IEnumerable<string>>
        {
            public NotificationProviderMapSetting()
                : base(string.Empty, new List<string>())
            {
            }

            public NotificationProviderMapSetting(string clientName, IEnumerable<string> providers)
                : base(clientName, providers)
            {
            }
        }
    }
}
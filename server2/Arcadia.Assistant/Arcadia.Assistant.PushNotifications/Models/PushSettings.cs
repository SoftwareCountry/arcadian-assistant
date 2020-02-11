﻿namespace Arcadia.Assistant.PushNotifications.Models
{
    using System.Fabric.Description;

    using Interfaces;

    public class PushSettings : IPushSettings
    {
        public PushSettings(ConfigurationSection configurationSection)
        {
            this.ApiToken = configurationSection.Parameters["ApiToken"].Value;
            this.AndroidPushUrl = configurationSection.Parameters["AndroidPushUrl"].Value;
            this.IosPushUrl = configurationSection.Parameters["IosPushUrl"].Value;
        }

        public string ApiToken { get; set; }

        public string AndroidPushUrl { get; set; }

        public string IosPushUrl { get; set; }
    }
}
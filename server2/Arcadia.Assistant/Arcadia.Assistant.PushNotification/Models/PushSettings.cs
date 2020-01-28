﻿using Arcadia.Assistant.PushNotification.Interfaces;

namespace Arcadia.Assistant.PushNotification.Models
{
    public class PushSettings : IPushSettings
    {
        public bool Enabled { get; set; }

        public string ApiToken { get; set; }

        public string AndroidPushUrl { get; set; }

        public string IosPushUrl { get; set; }
    }
}
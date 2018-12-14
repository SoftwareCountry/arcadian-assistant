namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;

    public class PushSettings : IPushSettings
    {
        public bool Enabled { get; set; }

        public string ApiToken { get; set; }

        public IEnumerable<string> ApplicationPushUrls { get; set; }
    }
}
using Microsoft.Extensions.Configuration;

namespace Arcadia.Assistant.Configuration
{
    using System.Collections.Generic;
    using System.IO;

    internal class HoconContentProvider : FileConfigurationProvider
    {
        private readonly string configKey;

        public HoconContentProvider(HoconContentSource hoconContentSource, string configKey)
            :base(hoconContentSource)
        {
            this.configKey = configKey;
        }

        public override void Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                this.Data = new Dictionary<string, string> { {this.configKey, reader.ReadToEnd() } };
            }
        }
    }
}
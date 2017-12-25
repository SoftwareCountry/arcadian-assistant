namespace Arcadia.Assistant.Configuration
{
    using System.IO;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Memory;
    using Microsoft.Extensions.FileProviders;

    public class HoconContentSource : FileConfigurationSource
    {
        private readonly string configKey;

        public HoconContentSource(string configKey)
        {
            this.configKey = configKey;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.FileProvider = this.FileProvider ?? builder.GetFileProvider();
            return new HoconContentProvider(this, this.configKey);
        }
    }
}